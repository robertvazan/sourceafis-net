using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Visualization;
using SourceAFIS.Extraction.Filters;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.Extraction
{
    public sealed class Extractor
    {
        [DpiAdjusted]
        public int BlockSize = 16;

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
        public TailRemover TailRemover = new TailRemover();
        [Nested]
        public FragmentRemover FragmentRemover = new FragmentRemover();

        public Extractor()
        {
            OrthogonalSmoother.AngleOffset = Angle.PIB;
            OrthogonalSmoother.Lines.Radius = 7;
            BinarySmoother.Radius = 2;
            BinarySmoother.Majority = 0.8f;
        }

        public void Extract(byte[,] invertedImage, int dpi)
        {
            DpiAdjuster.Adjust(this, dpi, delegate()
            {
                byte[,] image = GrayscaleInverter.GetInverted(invertedImage);

                BlockMap blocks = new BlockMap(new Size(image.GetLength(1), image.GetLength(0)), BlockSize);
                Logger.Log(this, "BlockMap", blocks);

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
                
                Threader.Ticket ridgeTicket = Threader.Schedule(delegate() { ProcessSkeleton("Ridges", binary, innerMask); });
                Threader.Ticket valleyTicket = Threader.Schedule(delegate() { ProcessSkeleton("Valleys", inverted, innerMask); });
                ridgeTicket.Wait();
                valleyTicket.Wait();
            });
        }

        void ProcessSkeleton(string name, BinaryMap binary, BinaryMap innerMask)
        {
            Logger.RunInContext(name, delegate()
            {
                Logger.Log(this, "Binarized", binary);
                BinaryMap thinned = Thinner.Thin(binary);
                SkeletonBuilder skeleton = new SkeletonBuilder();
                RidgeTracer.Trace(thinned, skeleton);
                DotRemover.Filter(skeleton);
                PoreRemover.Filter(skeleton);
                TailRemover.Filter(skeleton);
                FragmentRemover.Filter(skeleton);
                MinutiaMask.Filter(skeleton, innerMask);
            });
        }
    }
}
