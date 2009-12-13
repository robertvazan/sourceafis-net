using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Visualization;

namespace SourceAFIS.Extraction
{
    public class Extractor
    {
        [DpiAdjusted]
        public int BlockSize = 16;

        public DpiAdjuster DpiAdjuster = new DpiAdjuster();
        [Nested]
        public LocalHistogram Histogram = new LocalHistogram();
        [Nested]
        public Equalizer Equalizer = new Equalizer();

        public void Extract(byte[,] invertedImage, int dpi)
        {
            DpiAdjuster.Adjust(this, dpi, delegate()
            {
                byte[,] image = GrayscaleInverter.GetInverted(invertedImage);

                BlockMap blocks = new BlockMap();
                blocks.PixelCount = new Size(image.GetLength(1), image.GetLength(0));
                blocks.Initialize(BlockSize);

                short[, ,] histogram = Histogram.Analyze(blocks, image);
                histogram = Histogram.Smooth(blocks, histogram);
                float[,] equalized = Equalizer.Equalize(blocks, image, histogram);
            });
        }
    }
}
