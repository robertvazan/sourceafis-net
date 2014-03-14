using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;
using SourceAFIS.Extraction.Filters;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Minutiae;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction
{
    public sealed class Extractor
    {
        const int BlockSize = 15;

        public LocalHistogram Histogram = new LocalHistogram();
        public SegmentationMask Mask = new SegmentationMask();
        public Equalizer Equalizer = new Equalizer();
        public HillOrientation Orientation = new HillOrientation();
        public OrientedSmoother RidgeSmoother = new OrientedSmoother(lines: new LinesByOrientation(step: 1.59f));
        public OrientedSmoother OrthogonalSmoother = new OrientedSmoother(
            angle: Angle.PIB, lines: new LinesByOrientation(resolution: 11, radius: 4, step: 1.11f));
        public ThresholdBinarizer Binarizer = new ThresholdBinarizer();
        public VotingFilter BinarySmoother = new VotingFilter(radius: 2, majority: 0.61f, borderDist: 17);
        public Thinner Thinner = new Thinner();
        public CrossRemover CrossRemover = new CrossRemover();
        public RidgeTracer RidgeTracer = new RidgeTracer();
        public InnerMask InnerMask = new InnerMask();
        public MinutiaMask MinutiaMask = new MinutiaMask();
        public DotRemover DotRemover = new DotRemover();
        public PoreRemover PoreRemover = new PoreRemover();
        public GapRemover GapRemover = new GapRemover();
        public TailRemover TailRemover = new TailRemover();
        public FragmentRemover FragmentRemover = new FragmentRemover();
        public BranchMinutiaRemover BranchMinutiaRemover = new BranchMinutiaRemover();
        public MinutiaCollector MinutiaCollector = new MinutiaCollector();
        public MinutiaShuffler MinutiaSorter = new MinutiaShuffler();
        public MinutiaCloudRemover MinutiaCloudRemover = new MinutiaCloudRemover();
        public UniqueMinutiaSorter UniqueMinutiaSorter = new UniqueMinutiaSorter();

        public void Extract(byte[,] invertedImage, FingerprintTemplate template)
        {
            byte[,] image = ImageInverter.GetInverted(invertedImage);

            BlockMap blocks = new BlockMap(new Size(image.GetLength(1), image.GetLength(0)), BlockSize);

            short[, ,] histogram = Histogram.Analyze(blocks, image);
            short[, ,] smoothHistogram = Histogram.SmoothAroundCorners(blocks, histogram);
            BinaryMap mask = Mask.ComputeMask(blocks, histogram);
            float[,] equalized = Equalizer.Equalize(blocks, image, smoothHistogram, mask);

            byte[,] orientation = Orientation.Detect(equalized, mask, blocks);
            float[,] smoothed = RidgeSmoother.Smooth(equalized, orientation, mask, blocks);
            float[,] orthogonal = OrthogonalSmoother.Smooth(smoothed, orientation, mask, blocks);

            BinaryMap binary = Binarizer.Binarize(smoothed, orthogonal, mask, blocks);
            binary.AndNot(BinarySmoother.Filter(binary.GetInverted()));
            binary.Or(BinarySmoother.Filter(binary));
            CrossRemover.Remove(binary);

            BinaryMap pixelMask = mask.FillBlocks(blocks);
            BinaryMap innerMask = InnerMask.Compute(pixelMask);

            BinaryMap inverted = binary.GetInverted();
            inverted.And(pixelMask);

            SkeletonBuilder ridges = ProcessSkeleton("Ridges", binary);
            SkeletonBuilder valleys = ProcessSkeleton("Valleys", inverted);

            MinutiaCollector.Collect(ridges, MinutiaType.Ending, template);
            MinutiaCollector.Collect(valleys, MinutiaType.Bifurcation, template);
            MinutiaMask.Filter(template, innerMask);
            MinutiaCloudRemover.Filter(template);
            UniqueMinutiaSorter.Filter(template);
            MinutiaSorter.Shuffle(template);
        }

        SkeletonBuilder ProcessSkeleton(string name, BinaryMap binary)
        {
            SkeletonBuilder skeleton = null;
            BinaryMap thinned = Thinner.Thin(binary);
            skeleton = new SkeletonBuilder();
            RidgeTracer.Trace(thinned, skeleton);
            DotRemover.Filter(skeleton);
            PoreRemover.Filter(skeleton);
            GapRemover.Filter(skeleton);
            TailRemover.Filter(skeleton);
            FragmentRemover.Filter(skeleton);
            BranchMinutiaRemover.Filter(skeleton);
            return skeleton;
        }
    }
}
