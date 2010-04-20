using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Visualization;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class Equalizer
    {
        [Parameter(Lower = 1, Upper = 10)]
        public float MaxScaling = 4f;
        [Parameter(Lower = 0.1)]
        public float MinScaling = 0.25f;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        const float RangeMin = -1;
        const float RangeMax = 1;
        const float RangeSize = RangeMax - RangeMin;

        float[, ,] ComputeEqualization(BlockMap blocks, short[, ,] histogram, BinaryMap blockMask)
        {
            float widthMax = RangeSize / 256f * MaxScaling;
            float widthMin = RangeSize / 256f * MinScaling;

            float[] limitedMin = new float[256];
            float[] limitedMax = new float[256];
            for (int i = 0; i < 256; ++i)
            {
                limitedMin[i] = Math.Max(i * widthMin + RangeMin, RangeMax - (255 - i) * widthMax);
                limitedMax[i] = Math.Min(i * widthMax + RangeMin, RangeMax - (255 - i) * widthMin);
            }

            float[, ,] equalization = new float[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            Threader.Split<Point>(blocks.AllCorners, delegate(Point corner)
            {
                if (blockMask.GetBitSafe(corner.X, corner.Y, false)
                    || blockMask.GetBitSafe(corner.X - 1, corner.Y, false)
                    || blockMask.GetBitSafe(corner.X, corner.Y - 1, false)
                    || blockMask.GetBitSafe(corner.X - 1, corner.Y - 1, false))
                {
                    int area = 0;
                    for (int i = 0; i < 256; ++i)
                        area += histogram[corner.Y, corner.X, i];
                    float widthWeigth = RangeSize / area;

                    float top = RangeMin;
                    for (int i = 0; i < 256; ++i)
                    {
                        float width = histogram[corner.Y, corner.X, i] * widthWeigth;
                        float equalized = top + PixelFormat.ToFloat((byte)i) * width;
                        top += width;

                        float limited = equalized;
                        if (limited < limitedMin[i])
                            limited = limitedMin[i];
                        if (limited > limitedMax[i])
                            limited = limitedMax[i];
                        equalization[corner.Y, corner.X, i] = limited;
                    }
                }
            });
            return equalization;
        }

        float[,] PerformEqualization(BlockMap blocks, byte[,] image, float[, ,] equalization, BinaryMap blockMask)
        {
            float[,] result = new float[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.AllBlocks, delegate(Point block)
            {
                if (blockMask.GetBit(block))
                {
                    RectangleC area = blocks.BlockAreas[block];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                        {
                            byte pixel = image[y, x];

                            float bottomLeft = equalization[block.Y, block.X, pixel];
                            float bottomRight = equalization[block.Y, block.X + 1, pixel];
                            float topLeft = equalization[block.Y + 1, block.X, pixel];
                            float topRight = equalization[block.Y + 1, block.X + 1, pixel];

                            PointF fraction = area.GetFraction(new Point(x, y));
                            result[y, x] = Calc.Interpolate(topLeft, topRight, bottomLeft, bottomRight, fraction);
                        }
                }
            });
            Logger.Log(result);
            return result;
        }

        public float[,] Equalize(BlockMap blocks, byte[,] image, short[, ,] histogram, BinaryMap blockMask)
        {
            float[, ,] equalization = ComputeEqualization(blocks, histogram, blockMask);
            return PerformEqualization(blocks, image, equalization, blockMask);
        }
    }
}
