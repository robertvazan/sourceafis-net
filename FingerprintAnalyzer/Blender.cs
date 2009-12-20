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

        public struct ExtractionOptions
        {
            public bool OriginalImage;
            public bool Equalized;
            public bool SmoothedRidges;
            public bool OrthogonalSmoothing;
            public bool Binarized;
            public bool BinarySmoothing;
            public bool Contrast;
            public bool AbsoluteContrast;
            public bool RelativeContrast;
            public bool LowContrastMajority;
            public bool SegmentationMask;
            public bool Orientation;
            public bool Thinned;
        }

        public ExtractionOptions Probe;

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

            if (Probe.Thinned)
            {
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.ThinValleys, ColorF.Transparent, ColorF.Red));
                AlphaLayering.Layer(output, ScalarColoring.Mask(Logs.Probe.ThinRidges, ColorF.Transparent, ColorF.Green));
            }

            OutputImage = ImageIO.CreateBitmap(PixelFormat.ToColorB(output));
        }

        ColorF[,] BaseGrayscale(float[,] image)
        {
            GrayscaleInverter.Invert(image);
            GlobalContrast.Normalize(image);
            return PixelFormat.ToColorF(image);
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
