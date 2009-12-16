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
            public bool Contrast;
            public bool AbsoluteContrast;
            public bool RelativeContrast;
            public bool LowContrastMajority;
            public bool SegmentationMask;
        }

        public ExtractionOptions Probe;

        public Bitmap OutputImage;

        readonly ColorF TransparentRed = new ColorF(1, 0, 0, 0.25f);
        readonly ColorF TransparentGreen = new ColorF(0, 1, 0, 0.25f);
        readonly ColorF LightFog = new ColorF(0.9f, 0.9f, 0.9f, 0.9f);

        public void Blend()
        {
            ColorF[,] output;
            if (Probe.Equalized)
            {
                GrayscaleInverter.Invert(Logs.Probe.Equalized);
                GlobalContrast.Normalize(Logs.Probe.Equalized);
                output = PixelFormat.ToColorF(Logs.Probe.Equalized);
            }
            else if (Probe.OriginalImage)
                output = PixelFormat.ToColorF(Logs.Probe.InputImage);
            else
            {
                output = new ColorF[Logs.Probe.InputImage.GetLength(0), Logs.Probe.InputImage.GetLength(1)];
                for (int y = 0; y < output.GetLength(0); ++y)
                    for (int x = 0; x < output.GetLength(1); ++x)
                        output[y, x] = new ColorF(1, 1, 1, 1);
            }

            if (Probe.Contrast)
            {
                float[,] contrast = BlockFiller.FillBlocks(PixelFormat.ToFloat(Logs.Probe.BlockContrast), Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Interpolate(contrast, TransparentRed, TransparentGreen));
            }
            LayerMask(Probe.AbsoluteContrast, output, Logs.Probe.AbsoluteContrast, TransparentRed);
            LayerMask(Probe.RelativeContrast, output, Logs.Probe.RelativeContrast, TransparentRed);
            LayerMask(Probe.LowContrastMajority, output, Logs.Probe.LowContrastMajority, TransparentRed);

            Logs.Probe.SegmentationMask.Invert();
            LayerMask(Probe.SegmentationMask, output, Logs.Probe.SegmentationMask, LightFog);

            OutputImage = ImageIO.CreateBitmap(PixelFormat.ToColorB(output));
        }

        void LayerMask(bool condition, ColorF[,] output, BinaryMap mask, ColorF color)
        {
            if (condition)
            {
                BinaryMap scaled = BlockFiller.FillBlocks(mask, Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Mask(scaled, ColorF.Transparent, color));
            }
        }
    }
}
