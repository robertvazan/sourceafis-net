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
            blocks.InitCornerList();
            blocks.InitCornerAreas();
            float[, ,] equalization = new float[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            foreach (Point corner in blocks.CornerList)
            {
                float widthLimit = ScalingLimit / 256f * (UpperEqualizerBound - LowerEqualizerBound);
                int area = blocks.CornerAreas[corner.Y, corner.X].TotalArea;
                float top = LowerEqualizerBound;
                for (int i = 0; i < 256; ++i)
                {
                    float width = histogram[corner.Y, corner.X, i] * ((UpperEqualizerBound - LowerEqualizerBound) / area);
                    if (width > widthLimit)
                        width = widthLimit;
                    equalization[corner.Y, corner.X, i] = top + width / 2;
                    top += width;
                }
            }
            return equalization;
        }

        float[,] PerformEqualization(BlockMap blocks, byte[,] image, float[, ,] equalization)
        {
            blocks.InitBlockList();
            blocks.InitBlockAreas();
            float[,] result = new float[blocks.PixelCount.Height, blocks.PixelCount.Width];
            foreach (Point block in blocks.BlockList)
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
            }
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
