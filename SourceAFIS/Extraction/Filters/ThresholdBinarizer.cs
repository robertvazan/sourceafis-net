using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
using System.Threading.Tasks;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class ThresholdBinarizer
    {
        public DetailLogger.Hook Logger = DetailLogger.Null;

        public BinaryMap Binarize(float[,] input, float[,] baseline, BinaryMap mask, BlockMap blocks)
        {
            BinaryMap binarized = new BinaryMap(input.GetLength(1), input.GetLength(0));
            Parallel.ForEach(blocks.AllBlocks, delegate(Point block)
            {
                if (mask.GetBit(block))
                {
                    RectangleC rect = blocks.BlockAreas[block];
                    for (int y = rect.Bottom; y < rect.Top; ++y)
                        for (int x = rect.Left; x < rect.Right; ++x)
                            if (input[y, x] - baseline[y, x] > 0)
                                binarized.SetBitOne(x, y);
                }
            });
            Logger.Log(binarized);
            return binarized;
        }
    }
}
