using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Threading.Tasks;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class ClippedContrast
    {
        [Parameter(Upper = 0.4)]
        public float ClipFraction = 0.1f;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public byte[,] Compute(BlockMap blocks, short[, ,] histogram)
        {
            byte[,] result = new byte[blocks.BlockCount.Height, blocks.BlockCount.Width];
            Parallel.ForEach(blocks.AllBlocks, delegate(Point block)
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
            });
            Logger.Log(result);
            return result;
        }
    }
}
