using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public sealed class LocalHistogram
    {
        public short[, ,] Analyze(BlockMap blocks, byte[,] image)
        {
            short[, ,] histogram = new short[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            Threader.Split<Point>(blocks.CornerList, delegate(Point corner)
            {
                RectangleC area = blocks.CornerAreas[corner.Y, corner.X];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        ++histogram[corner.Y, corner.X, image[y, x]];
            });
            return histogram;
        }

        public short[, ,] Smooth(BlockMap blocks, short[, ,] input)
        {
            short[, ,] output = new short[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            Threader.Split<Point>(blocks.CornerList, delegate(Point corner)
            {
                for (int i = 0; i < 256; ++i)
                    output[corner.Y, corner.X, i] = input[corner.Y, corner.X, i];
                foreach (Point neigborRelative in Neighborhood.CornerNeighbors)
                {
                    Point neighbor = Calc.Add(corner, neigborRelative);
                    if (blocks.CornerRect.Contains(neighbor))
                    {
                        for (int i = 0; i < 256; ++i)
                            output[corner.Y, corner.X, i] += input[neighbor.Y, neighbor.X, i];
                    }
                }
            });
            return output;
        }
    }
}
