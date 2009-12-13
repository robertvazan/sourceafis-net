using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public class Equalizer
    {
        public float ScalingLimit = 4f;

        const float LowerEqualizerBound = -1f;
        const float UpperEqualizerBound = 1f;

        float[, ,] ComputeEqualization(BlockMap blocks, short[, ,] histogram)
        {
            float[, ,] equalization = new float[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            Threader.Split<Point>(blocks.CornerList, delegate(Point corner)
            {
                float widthLimit = ScalingLimit / 256f * (UpperEqualizerBound - LowerEqualizerBound);
                int area = 0;
                for (int i = 0; i < 256; ++i)
                    area += histogram[corner.Y, corner.X, i];
                float top = LowerEqualizerBound;
                for (int i = 0; i < 256; ++i)
                {
                    float width = histogram[corner.Y, corner.X, i] * ((UpperEqualizerBound - LowerEqualizerBound) / area);
                    if (width > widthLimit)
                        width = widthLimit;
                    equalization[corner.Y, corner.X, i] = top + width / 2;
                    top += width;
                }
            });
            return equalization;
        }

        float[,] PerformEqualization(BlockMap blocks, byte[,] image, float[, ,] equalization)
        {
            float[,] result = new float[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.BlockList, delegate(Point block)
            {
                RectangleC area = blocks.BlockAreas[block.Y, block.X];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                    {
                        float bottomLeft = equalization[block.Y, block.X, image[y, x]];
                        float bottomRight = equalization[block.Y, block.X + 1, image[y, x]];
                        float topLeft = equalization[block.Y + 1, block.X, image[y, x]];
                        float topRight = equalization[block.Y + 1, block.X + 1, image[y, x]];

                        PointF fraction = area.GetFraction(new Point(x, y));
                        result[y, x] = Calc.Interpolate(topLeft, topRight, bottomLeft, bottomRight, fraction);
                    }
            });
            Logger.Log(this, result);
            return result;
        }

        public float[,] Equalize(BlockMap blocks, byte[,] image, short[, ,] histogram)
        {
            float[, ,] equalization = ComputeEqualization(blocks, histogram);
            return PerformEqualization(blocks, image, equalization);
        }
    }
}
