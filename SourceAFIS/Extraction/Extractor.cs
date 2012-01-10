using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Threading.Tasks;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Filters;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Extraction
{
    public sealed class Extractor
    {
        [DpiAdjusted]
        [Parameter(Lower = 8, Upper = 32)]
        public int BlockSize = 15;

        public DpiAdjuster DpiAdjuster = new DpiAdjuster();
        [Nested]
        public LocalHistogram Histogram = new LocalHistogram();
        [Nested]
        public SegmentationMask Mask = new SegmentationMask();
        [Nested]
        public Equalizer Equalizer = new Equalizer();
        [Nested]
        public HillOrientation Orientation = new HillOrientation();
        [Nested]
        public OrientedSmoother RidgeSmoother = new OrientedSmoother();
        [Nested]
        public OrientedSmoother OrthogonalSmoother = new OrientedSmoother();
        [Nested]
        public ThresholdBinarizer Binarizer = new ThresholdBinarizer();
        [Nested]
        public VotingFilter BinarySmoother = new VotingFilter();
        [Nested]
        public Thinner Thinner = new Thinner();
        [Nested]
        public CrossRemover CrossRemover = new CrossRemover();
        [Nested]
        public RidgeTracer RidgeTracer = new RidgeTracer();
        [Nested]
        public InnerMask InnerMask = new InnerMask();
        [Nested]
        public MinutiaMask MinutiaMask = new MinutiaMask();
        [Nested]
        public DotRemover DotRemover = new DotRemover();
        [Nested]
        public PoreRemover PoreRemover = new PoreRemover();
        [Nested]
        public GapRemover GapRemover = new GapRemover();
        [Nested]
        public TailRemover TailRemover = new TailRemover();
        [Nested]
        public FragmentRemover FragmentRemover = new FragmentRemover();
        [Nested]
        public BranchMinutiaRemover BranchMinutiaRemover = new BranchMinutiaRemover();
        [Nested]
        public MinutiaCollector MinutiaCollector = new MinutiaCollector();
        [Nested]
        public MinutiaSorter MinutiaSorter = new MinutiaSorter();

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public Extractor()
        {
            RidgeSmoother.Lines.StepFactor = 1.59f;
            OrthogonalSmoother.AngleOffset = Angle.PIB;
            OrthogonalSmoother.Lines.Radius = 4;
            BinarySmoother.Radius = 2;
            BinarySmoother.Majority = 0.66f;
            BinarySmoother.BorderDistance = 17;
        }

        public TemplateBuilder Extract(byte[,] invertedImage, int dpi)
        {
            TemplateBuilder template = null;
            DpiAdjuster.Adjust(this, dpi, delegate()
            {
                byte[,] image = ImageInverter.GetInverted(invertedImage);

                BlockMap blocks = new BlockMap(new Size(image.GetLength(1), image.GetLength(0)), BlockSize);
                Logger.Log("BlockMap", blocks);

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
                Logger.Log("BinarySmoothingResult", binary);
                CrossRemover.Remove(binary);

                BinaryMap pixelMask = mask.FillBlocks(blocks);
                BinaryMap innerMask = InnerMask.Compute(pixelMask);

                BinaryMap inverted = binary.GetInverted();
                inverted.And(pixelMask);

                SkeletonBuilder ridges = null;
                SkeletonBuilder valleys = null;
                Parallel.Invoke(
                    () => { ridges = ProcessSkeleton("Ridges", binary, innerMask); },
                    () => { valleys = ProcessSkeleton("Valleys", inverted, innerMask); });

                template = new TemplateBuilder();
                template.OriginalDpi = dpi;
                template.OriginalWidth = invertedImage.GetLength(1);
                template.OriginalHeight = invertedImage.GetLength(0);

                MinutiaCollector.Collect(ridges, TemplateBuilder.MinutiaType.Ending, template);
                MinutiaCollector.Collect(valleys, TemplateBuilder.MinutiaType.Bifurcation, template);
                MinutiaSorter.Sort(template);
            });
            return template;
        }

        SkeletonBuilder ProcessSkeleton(string name, BinaryMap binary, BinaryMap innerMask)
        {
            SkeletonBuilder skeleton = null;
            DetailLogger.RunInContext(name, delegate()
            {
                Logger.Log("Binarized", binary);
                BinaryMap thinned = Thinner.Thin(binary);
                skeleton = new SkeletonBuilder();
                RidgeTracer.Trace(thinned, skeleton);
                DotRemover.Filter(skeleton);
                PoreRemover.Filter(skeleton);
                GapRemover.Filter(skeleton);
                TailRemover.Filter(skeleton);
                FragmentRemover.Filter(skeleton);
                MinutiaMask.Filter(skeleton, innerMask);
                BranchMinutiaRemover.Filter(skeleton);
            });
            return skeleton;
        }
    }
}
