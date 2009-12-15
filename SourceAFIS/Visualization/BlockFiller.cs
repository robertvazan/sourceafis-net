using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public class BlockFiller
    {
        public static T[,] FillBlocks<T>(T[,] input, BlockMap blocks) where T : struct
        {
            T[,] output = new T[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.BlockList, delegate(Point block)
            {
                T fill = input[block.Y, block.X];
                RectangleC area = blocks.BlockAreas[block.Y, block.X];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        output[y, x] = fill;
            });
            return output;
        }

        public static T[,] FillCornerAreas<T>(T[,] input, BlockMap blocks) where T : struct
        {
            T[,] output = new T[blocks.PixelCount.Height, blocks.PixelCount.Width];
            Threader.Split<Point>(blocks.CornerList, delegate(Point corner)
            {
                T fill = input[corner.Y, corner.X];
                RectangleC area = blocks.CornerAreas[corner.Y, corner.X];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        output[y, x] = fill;
            });
            return output;
        }

        public static BinaryMap FillBlocks(BinaryMap input, BlockMap blocks)
        {
            BinaryMap output = new BinaryMap(blocks.PixelCount.Width, blocks.PixelCount.Height);
            foreach (Point block in blocks.BlockList)
            {
                if (input.GetBit(block))
                {
                    RectangleC area = blocks.BlockAreas[block.Y, block.X];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            output.SetBitOne(x, y);
                }
            }
            return output;
        }

        public static BinaryMap FillCornerAreas(BinaryMap input, BlockMap blocks)
        {
            BinaryMap output = new BinaryMap(blocks.PixelCount.Width, blocks.PixelCount.Height);
            foreach (Point corner in blocks.CornerList)
            {
                if (input.GetBit(corner))
                {
                    RectangleC area = blocks.CornerAreas[corner.Y, corner.X];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            output.SetBitOne(x, y);
                }
            }
            return output;
        }
    }
}
