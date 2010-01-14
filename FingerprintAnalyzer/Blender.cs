using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class Blender
    {
        public LogCollector Logs;

        public sealed class SkeletonOptions
        {
            public bool Binarized;
            public bool Thinned;
            public bool RidgeTracer;
            public bool DotRemover;
            public bool PoreRemover;
            public bool TailRemover;
            public bool FragmentRemover;
            public bool MinutiaMask;
            public bool ShowEndings;
        }

        public sealed class ExtractionOptions
        {
            public bool OriginalImage;
            public bool Equalized;
            public bool SmoothedRidges;
            public bool OrthogonalSmoothing;
            public bool Contrast;
            public bool AbsoluteContrast;
            public bool RelativeContrast;
            public bool LowContrastMajority;
            public bool SegmentationMask;
            public bool Orientation;
            public bool Binarized;
            public bool BinarySmoothing;
            public bool RemovedCrosses;
            public bool Thinned;
            public bool InnerMask;
            public SkeletonOptions Ridges = new SkeletonOptions();
            public SkeletonOptions Valleys = new SkeletonOptions();
            public bool MinutiaCollector;
        }

        public ExtractionOptions Probe = new ExtractionOptions();

        public Bitmap OutputImage;

        readonly ColorF TransparentRed = new ColorF(1, 0, 0, 0.25f);
        readonly ColorF TransparentGreen = new ColorF(0, 1, 0, 0.25f);
        readonly ColorF LightFog = new ColorF(0.9f, 0.9f, 0.9f, 0.9f);

        public void Blend()
        {
            ColorF[,] output;
            bool empty = false;
            if (Probe.OrthogonalSmoothing)
                output = BaseGrayscale(Logs.Probe.OrthogonalSmoothing);
            else if (Probe.SmoothedRidges)
                output = BaseGrayscale(Logs.Probe.SmoothedRidges);
            else if (Probe.Equalized)
                output = BaseGrayscale(Logs.Probe.Equalized);
            else if (Probe.OriginalImage)
                output = PixelFormat.ToColorF(Logs.Probe.InputImage);
            else
            {
                output = new ColorF[Logs.Probe.InputImage.GetLength(0), Logs.Probe.InputImage.GetLength(1)];
                for (int y = 0; y < output.GetLength(0); ++y)
                    for (int x = 0; x < output.GetLength(1); ++x)
                        output[y, x] = new ColorF(1, 1, 1, 1);
                empty = true;
            }

            if (Probe.Binarized)
            {
                if (empty)
                {
                    output = ScalarColoring.Mask(Logs.Probe.Binarized, ColorF.White, ColorF.Black);
                    empty = false;
                }
                else
                    AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Binarized, ColorF.Transparent, TransparentGreen));
            }
            if (Probe.BinarySmoothing)
            {
                Logs.Probe.BinarySmoothingZeroes.And(Logs.Probe.Binarized);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.BinarySmoothingZeroes, ColorF.Transparent, ColorF.Red));
                Logs.Probe.BinarySmoothingOnes.AndNot(Logs.Probe.Binarized);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.BinarySmoothingOnes, ColorF.Transparent, ColorF.Green));
            }
            if (Probe.RemovedCrosses)
            {
                Logs.Probe.RemovedCrosses.Invert();
                BinaryMap smoothedBinary = new BinaryMap(Logs.Probe.Binarized);
                smoothedBinary.AndNot(Logs.Probe.BinarySmoothingZeroes);
                smoothedBinary.Or(Logs.Probe.BinarySmoothingOnes);
                Logs.Probe.RemovedCrosses.And(smoothedBinary);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.RemovedCrosses, ColorF.Transparent, ColorF.Red));
            }

            LayerBlocks(Probe.Contrast, output, PixelFormat.ToFloat(Logs.Probe.BlockContrast));
            LayerMask(Probe.AbsoluteContrast, output, Logs.Probe.AbsoluteContrast, TransparentRed);
            LayerMask(Probe.RelativeContrast, output, Logs.Probe.RelativeContrast, TransparentRed);
            LayerMask(Probe.LowContrastMajority, output, Logs.Probe.LowContrastMajority, TransparentRed);

            if (Probe.Orientation)
            {
                BinaryMap markers = OrientationMarkers.Draw(Logs.Probe.Orientation, Logs.Probe.Blocks, Logs.Probe.SegmentationMask);
                AlphaLayering.Layer(output, ScalarColoring.Mask(markers, ColorF.Transparent, ColorF.Red));
            }

            Logs.Probe.SegmentationMask.Invert();
            LayerMask(Probe.SegmentationMask, output, Logs.Probe.SegmentationMask, LightFog);
            if (Probe.InnerMask)
            {
                Logs.Probe.InnerMask.Invert();
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.InnerMask, ColorF.Transparent, LightFog));
            }

            if (Probe.Thinned)
            {
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Valleys.Thinned, ColorF.Transparent, ColorF.Red));
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Ridges.Thinned, ColorF.Transparent, ColorF.Green));
            }

            RenderSkeleton(output, Probe.Ridges, Logs.Probe.Ridges);
            RenderSkeleton(output, Probe.Valleys, Logs.Probe.Valleys);

            if (Probe.MinutiaCollector)
                TemplateDrawer.Draw(output, Logs.Probe.MinutiaCollector);

            OutputImage = ImageIO.CreateBitmap(PixelFormat.ToColorB(output));
        }

        void RenderSkeleton(ColorF[,] output, SkeletonOptions options, LogCollector.SkeletonData logs)
        {
            if (options.Binarized)
                AlphaLayering.Layer(output, ScalarColoring.Mask(logs.Binarized, ColorF.White, ColorF.Black));
            if (options.Thinned)
                AlphaLayering.Layer(output, ScalarColoring.Mask(logs.Thinned, ColorF.Transparent, options.Binarized ? ColorF.Green : ColorF.Black));
            if (options.RidgeTracer)
                AlphaLayering.Layer(output, ScalarColoring.Mask(SkeletonDrawer.Draw(logs.RidgeTracer, logs.Binarized.Size), ColorF.Transparent, ColorF.Black));
            if (options.DotRemover)
                LayerBinaryDiff(output, SkeletonDrawer.Draw(logs.RidgeTracer, logs.Binarized.Size), SkeletonDrawer.Draw(logs.DotRemover, logs.Binarized.Size));
            if (options.PoreRemover)
                LayerBinaryDiff(output, SkeletonDrawer.Draw(logs.DotRemover, logs.Binarized.Size), SkeletonDrawer.Draw(logs.PoreRemover, logs.Binarized.Size));
            if (options.TailRemover)
                LayerBinaryDiff(output, SkeletonDrawer.Draw(logs.PoreRemover, logs.Binarized.Size), SkeletonDrawer.Draw(logs.TailRemover, logs.Binarized.Size));
            if (options.FragmentRemover)
                LayerBinaryDiff(output, SkeletonDrawer.Draw(logs.TailRemover, logs.Binarized.Size), SkeletonDrawer.Draw(logs.FragmentRemover, logs.Binarized.Size));
            if (options.MinutiaMask)
                LayerBinaryDiff(output, SkeletonDrawer.Draw(logs.FragmentRemover, logs.Binarized.Size), SkeletonDrawer.Draw(logs.MinutiaMask, logs.Binarized.Size));
            if (options.ShowEndings)
                AlphaLayering.Layer(output, ScalarColoring.Mask(SkeletonDrawer.DrawEndings(logs.MinutiaMask, logs.Binarized.Size), ColorF.Transparent, ColorF.Blue));
        }

        ColorF[,] BaseGrayscale(float[,] image)
        {
            GrayscaleInverter.Invert(image);
            GlobalContrast.Normalize(image);
            return PixelFormat.ToColorF(image);
        }

        void LayerBinaryDiff(ColorF[,] output, BinaryMap first, BinaryMap second)
        {
            BinaryMap removed = new BinaryMap(first);
            removed.AndNot(second);
            AlphaLayering.Layer(output, ScalarColoring.Mask(removed, ColorF.Transparent, ColorF.Red));
            BinaryMap added = new BinaryMap(second);
            added.AndNot(first);
            AlphaLayering.Layer(output, ScalarColoring.Mask(added, ColorF.Transparent, ColorF.Green));
        }

        void LayerMask(bool condition, ColorF[,] output, BinaryMap mask, ColorF color)
        {
            if (condition)
            {
                BinaryMap scaled = BlockFiller.FillBlocks(mask, Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Mask(scaled, ColorF.Transparent, color));
            }
        }

        void LayerBlocks(bool condition, ColorF[,] output, float[,] data)
        {
            if (condition)
            {
                GlobalContrast.Normalize(data);
                float[,] scaled = BlockFiller.FillBlocks(data, Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Interpolate(scaled, TransparentRed, TransparentGreen));
            }
        }
    }
}
