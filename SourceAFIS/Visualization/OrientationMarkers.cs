using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Extraction;

namespace SourceAFIS.Visualization
{
    public sealed class OrientationMarkers
    {
        public static BinaryMap Draw(byte[,] orientation, BlockMap blocks, BinaryMap mask)
        {
            BinaryMap output = new BinaryMap(blocks.PixelCount);
            foreach (Point block in blocks.BlockList)
            {
                if (mask.GetBit(block))
                {
                    PointF direction = Angle.ToVector(Angle.ToDirection(orientation[block.Y, block.X]));
                    int radius = (Math.Min(blocks.BlockAreas[block.Y, block.X].Width, blocks.BlockAreas[block.Y, block.X].Height) - 1) / 2;
                    for (int i = -radius; i <= radius; ++i)
                        output.SetBitOne(Calc.Add(blocks.BlockCenters[block.Y, block.X], Calc.Round(Calc.Multiply(i, direction))));
                }
            }
            return output;
        }
    }
}
