using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class BlockFiller
    {
        public static T[,] FillBlocks<T>(T[,] input, BlockMap blocks) where T : struct
        {
            T[,] output = new T[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.AllBlocks, delegate(Point block)
            {
                T fill = input[block.Y, block.X];
                RectangleC area = blocks.BlockAreas[block];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        output[y, x] = fill;
            });
            return output;
        }

        public static T[,] FillCornerAreas<T>(T[,] input, BlockMap blocks) where T : struct
        {
            T[,] output = new T[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.AllCorners, delegate(Point corner)
            {
                T fill = input[corner.Y, corner.X];
                RectangleC area = blocks.CornerAreas[corner];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        output[y, x] = fill;
            });
            return output;
        }

        public static BinaryMap FillBlocks(BinaryMap input, BlockMap blocks)
        {
            return input.FillBlocks(blocks);
        }

        public static BinaryMap FillCornerAreas(BinaryMap input, BlockMap blocks)
        {
            return input.FillCornerAreas(blocks);
        }
    }
}
