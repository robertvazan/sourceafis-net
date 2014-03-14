using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class ThresholdBinarizer
    {
        public BinaryMap Binarize(float[,] input, float[,] baseline, BinaryMap mask, BlockMap blocks)
        {
            BinaryMap binarized = new BinaryMap(input.GetLength(1), input.GetLength(0));
            for (int blockY = 0; blockY < blocks.AllBlocks.Height; ++blockY)
            {
                for (int blockX = 0; blockX < blocks.AllBlocks.Width; ++blockX)
                {
                    if (mask.GetBit(blockX, blockY))
                    {
                        RectangleC rect = blocks.BlockAreas[blockY, blockX];
                        for (int y = rect.Bottom; y < rect.Top; ++y)
                            for (int x = rect.Left; x < rect.Right; ++x)
                                if (input[y, x] - baseline[y, x] > 0)
                                    binarized.SetBitOne(x, y);
                    }
                }
            }
            return binarized;
        }
    }
}
