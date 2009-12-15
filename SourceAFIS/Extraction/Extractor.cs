using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Visualization;

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
        public Equalizer Equalizer = new Equalizer();
        [Nested]
        public ClippedContrast Contrast = new ClippedContrast();
        [Nested]
        public AbsoluteContrast AbsoluteContrast = new AbsoluteContrast();

        public void Extract(byte[,] invertedImage, int dpi)
        {
            DpiAdjuster.Adjust(this, dpi, delegate()
            {
                byte[,] image = GrayscaleInverter.GetInverted(invertedImage);

                BlockMap blocks = new BlockMap();
                blocks.PixelCount = new Size(image.GetLength(1), image.GetLength(0));
                blocks.Initialize(BlockSize);
                Logger.Log(this, "BlockMap", blocks);

                short[, ,] histogram = Histogram.Analyze(blocks, image);
                short[, ,] smoothHistogram = Histogram.Smooth(blocks, histogram);

                byte[,] contrast = Contrast.Compute(blocks, histogram);
                BinaryMap absolutContrastLow = AbsoluteContrast.DetectLowContrast(contrast);

                float[,] equalized = Equalizer.Equalize(blocks, image, smoothHistogram);
            });
        }
    }
}
