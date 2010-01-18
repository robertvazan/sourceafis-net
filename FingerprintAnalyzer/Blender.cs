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

        public Options Options = new Options();

        public Bitmap OutputImage;

        readonly ColorF TransparentRed = new ColorF(1, 0, 0, 0.25f);
        readonly ColorF TransparentGreen = new ColorF(0, 1, 0, 0.25f);
        readonly ColorF LightFog = new ColorF(0.9f, 0.9f, 0.9f, 0.9f);

        public void Blend()
        {
            ColorF[,] output;
            bool empty = false;
            if (Options.Probe.OrthogonalSmoothing)
                output = BaseGrayscale(Logs.Probe.OrthogonalSmoothing);
            else if (Options.Probe.SmoothedRidges)
                output = BaseGrayscale(Logs.Probe.SmoothedRidges);
            else if (Options.Probe.Equalized)
                output = BaseGrayscale(Logs.Probe.Equalized);
            else if (Options.Probe.OriginalImage)
                output = PixelFormat.ToColorF(Logs.Probe.InputImage);
            else
            {
                output = new ColorF[Logs.Probe.InputImage.GetLength(0), Logs.Probe.InputImage.GetLength(1)];
                for (int y = 0; y < output.GetLength(0); ++y)
                    for (int x = 0; x < output.GetLength(1); ++x)
                        output[y, x] = new ColorF(1, 1, 1, 1);
                empty = true;
            }

            if (Options.Probe.Binarized)
            {
                if (empty)
                {
                    output = ScalarColoring.Mask(Logs.Probe.Binarized, ColorF.White, ColorF.Black);
                    empty = false;
                }
                else
                    AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Binarized, ColorF.Transparent, TransparentGreen));
            }
            if (Options.Probe.BinarySmoothing)
            {
                Logs.Probe.BinarySmoothingZeroes.And(Logs.Probe.Binarized);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.BinarySmoothingZeroes, ColorF.Transparent, ColorF.Red));
                Logs.Probe.BinarySmoothingOnes.AndNot(Logs.Probe.Binarized);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.BinarySmoothingOnes, ColorF.Transparent, ColorF.Green));
            }
            if (Options.Probe.RemovedCrosses)
            {
                Logs.Probe.RemovedCrosses.Invert();
                BinaryMap smoothedBinary = new BinaryMap(Logs.Probe.Binarized);
                smoothedBinary.AndNot(Logs.Probe.BinarySmoothingZeroes);
                smoothedBinary.Or(Logs.Probe.BinarySmoothingOnes);
                Logs.Probe.RemovedCrosses.And(smoothedBinary);
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.RemovedCrosses, ColorF.Transparent, ColorF.Red));
            }

            LayerBlocks(Options.Probe.Contrast, output, PixelFormat.ToFloat(Logs.Probe.BlockContrast));
            LayerMask(Options.Probe.AbsoluteContrast, output, Logs.Probe.AbsoluteContrast, TransparentRed);
            LayerMask(Options.Probe.RelativeContrast, output, Logs.Probe.RelativeContrast, TransparentRed);
            LayerMask(Options.Probe.LowContrastMajority, output, Logs.Probe.LowContrastMajority, TransparentRed);

            if (Options.Probe.Orientation)
            {
                BinaryMap markers = OrientationMarkers.Draw(Logs.Probe.Orientation, Logs.Probe.Blocks, Logs.Probe.SegmentationMask);
                AlphaLayering.Layer(output, ScalarColoring.Mask(markers, ColorF.Transparent, ColorF.Red));
            }

            Logs.Probe.SegmentationMask.Invert();
            LayerMask(Options.Probe.SegmentationMask, output, Logs.Probe.SegmentationMask, LightFog);
            if (Options.Probe.InnerMask)
            {
                Logs.Probe.InnerMask.Invert();
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.InnerMask, ColorF.Transparent, LightFog));
            }

            if (Options.Probe.Thinned)
            {
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Valleys.Thinned, ColorF.Transparent, ColorF.Red));
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.Ridges.Thinned, ColorF.Transparent, ColorF.Green));
            }

            RenderSkeleton(output, Options.Probe.Ridges, Logs.Probe.Ridges);
            RenderSkeleton(output, Options.Probe.Valleys, Logs.Probe.Valleys);

            if (Options.Probe.MinutiaCollector)
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
