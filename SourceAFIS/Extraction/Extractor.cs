using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;
using SourceAFIS.Extraction.Filters;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Minutiae;

namespace SourceAFIS.Extraction
{
    public static class Extractor
    {
        const int BlockSize = 15;

        static readonly OrientedSmoother RidgeSmoother = new OrientedSmoother(lines: new LinesByOrientation(step: 1.59f));
        static readonly OrientedSmoother OrthogonalSmoother = new OrientedSmoother(
            angle: Angle.PIB, lines: new LinesByOrientation(resolution: 11, radius: 4, step: 1.11f));
        static readonly VotingFilter BinarySmoother = new VotingFilter(radius: 2, majority: 0.61f, borderDist: 17);

        public static void Extract(byte[,] invertedImage, FingerprintTemplate template)
        {
            byte[,] image = ImageInverter.GetInverted(invertedImage);

            BlockMap blocks = new BlockMap(new Size(image.GetLength(1), image.GetLength(0)), BlockSize);

            short[, ,] histogram = LocalHistogram.Analyze(blocks, image);
            short[, ,] smoothHistogram = LocalHistogram.SmoothAroundCorners(blocks, histogram);
            BinaryMap mask = SegmentationMask.ComputeMask(blocks, histogram);
            float[,] equalized = Equalizer.Equalize(blocks, image, smoothHistogram, mask);

            byte[,] orientation = HillOrientation.Detect(equalized, mask, blocks);
            float[,] smoothed = RidgeSmoother.Smooth(equalized, orientation, mask, blocks);
            float[,] orthogonal = OrthogonalSmoother.Smooth(smoothed, orientation, mask, blocks);

            BinaryMap binary = ThresholdBinarizer.Binarize(smoothed, orthogonal, mask, blocks);
            binary.AndNot(BinarySmoother.Filter(binary.GetInverted()));
            binary.Or(BinarySmoother.Filter(binary));
            CrossRemover.Remove(binary);

            BinaryMap pixelMask = mask.FillBlocks(blocks);
            BinaryMap innerMask = InnerMask.Compute(pixelMask);

            BinaryMap inverted = binary.GetInverted();
            inverted.And(pixelMask);

            SkeletonBuilder ridges = ProcessSkeleton("Ridges", binary);
            SkeletonBuilder valleys = ProcessSkeleton("Valleys", inverted);

            MinutiaCollector.Collect(ridges, FingerprintMinutiaType.Ending, template);
            MinutiaCollector.Collect(valleys, FingerprintMinutiaType.Bifurcation, template);
            MinutiaMask.Filter(template, innerMask);
            MinutiaCloudRemover.Filter(template);
            UniqueMinutiaSorter.Filter(template);
            MinutiaShuffler.Shuffle(template);
        }

        static SkeletonBuilder ProcessSkeleton(string name, BinaryMap binary)
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
