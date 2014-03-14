using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public static class ClippedContrast
    {
        const float ClipFraction = 0.08f;

        public static byte[,] Compute(BlockMap blocks, short[, ,] histogram)
        {
            byte[,] result = new byte[blocks.BlockCount.Height, blocks.BlockCount.Width];
            foreach (var block in blocks.AllBlocks)
            {
                int area = 0;
                for (int i = 0; i < 256; ++i)
                    area += histogram[block.Y, block.X, i];
                int clipLimit = Convert.ToInt32(area * ClipFraction);

                int accumulator = 0;
                int lowerBound = 255;
                for (int i = 0; i < 256; ++i)
                {
                    accumulator += histogram[block.Y, block.X, i];
                    if (accumulator > clipLimit)
                    {
                        lowerBound = i;
                        break;
                    }
                }

                accumulator = 0;
                int upperBound = 0;
                for (int i = 255; i >= 0; --i)
                {
                    accumulator += histogram[block.Y, block.X, i];
                    if (accumulator > clipLimit)
                    {
                        upperBound = i;
                        break;
                    }
                }

                result[block.Y, block.X] = (byte)(upperBound - lowerBound);
            }
            return result;
        }
    }
}
