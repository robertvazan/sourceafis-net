// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor
{
    static class SegmentationMask
    {
        static BooleanMatrix Filter(BooleanMatrix input)
        {
            return VoteFilter.Vote(input, null, Parameters.BlockErrorsVoteRadius, Parameters.BlockErrorsVoteMajority, Parameters.BlockErrorsVoteBorderDistance);
        }
        public static BooleanMatrix Compute(BlockMap blocks, HistogramCube histogram)
        {
            var contrast = ClippedContrast.Compute(blocks, histogram);
            var mask = AbsoluteContrastMask.Compute(contrast);
            mask.Merge(RelativeContrastMask.Compute(contrast, blocks));
            // https://sourceafis.machinezoo.com/transparency/combined-mask
            FingerprintTransparency.Current.Log("combined-mask", mask);
            mask.Merge(Filter(mask));
            mask.Invert();
            mask.Merge(Filter(mask));
            mask.Merge(Filter(mask));
            mask.Merge(VoteFilter.Vote(mask, null, Parameters.MaskVoteRadius, Parameters.MaskVoteMajority, Parameters.MaskVoteBorderDistance));
            // https://sourceafis.machinezoo.com/transparency/filtered-mask
            FingerprintTransparency.Current.Log("filtered-mask", mask);
            return mask;
        }
        public static BooleanMatrix Pixelwise(BooleanMatrix mask, BlockMap blocks)
        {
            var pixelized = new BooleanMatrix(blocks.Pixels);
            foreach (var block in blocks.Primary.Blocks.Iterate())
                if (mask[block])
                    foreach (var pixel in blocks.Primary.Block(block).Iterate())
                        pixelized[pixel] = true;
            return pixelized;
        }
        static BooleanMatrix Shrink(BooleanMatrix mask, int amount)
        {
            var size = mask.Size;
            var shrunk = new BooleanMatrix(size);
            for (int y = amount; y < size.Y - amount; ++y)
                for (int x = amount; x < size.X - amount; ++x)
                    shrunk[x, y] = mask[x, y - amount] && mask[x, y + amount] && mask[x - amount, y] && mask[x + amount, y];
            return shrunk;
        }
        public static BooleanMatrix Inner(BooleanMatrix outer)
        {
            var size = outer.Size;
            var inner = new BooleanMatrix(size);
            for (int y = 1; y < size.Y - 1; ++y)
                for (int x = 1; x < size.X - 1; ++x)
                    inner[x, y] = outer[x, y];
            if (Parameters.InnerMaskBorderDistance >= 1)
                inner = Shrink(inner, 1);
            int total = 1;
            for (int step = 1; total + step <= Parameters.InnerMaskBorderDistance; step *= 2)
            {
                inner = Shrink(inner, step);
                total += step;
            }
            if (total < Parameters.InnerMaskBorderDistance)
                inner = Shrink(inner, Parameters.InnerMaskBorderDistance - total);
            // https://sourceafis.machinezoo.com/transparency/inner-mask
            FingerprintTransparency.Current.Log("inner-mask", inner);
            return inner;
        }
    }
}
