// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor
{
    static class BinarizedImage
    {
        public static BooleanMatrix Binarize(DoubleMatrix input, DoubleMatrix baseline, BooleanMatrix mask, BlockMap blocks)
        {
            var size = input.Size;
            var binarized = new BooleanMatrix(size);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                if (mask[block])
                {
                    var rect = blocks.Primary.Block(block);
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                        for (int x = rect.Left; x < rect.Right; ++x)
                            if (input[x, y] - baseline[x, y] > 0)
                                binarized[x, y] = true;
                }
            }
            // https://sourceafis.machinezoo.com/transparency/binarized-image
            FingerprintTransparency.Current.Log("binarized-image", binarized);
            return binarized;
        }
        static void RemoveCrosses(BooleanMatrix input)
        {
            var size = input.Size;
            bool any = true;
            while (any)
            {
                any = false;
                for (int y = 0; y < size.Y - 1; ++y)
                    for (int x = 0; x < size.X - 1; ++x)
                        if (input[x, y] && input[x + 1, y + 1] && !input[x, y + 1] && !input[x + 1, y] || input[x, y + 1] && input[x + 1, y] && !input[x, y] && !input[x + 1, y + 1])
                        {
                            input[x, y] = false;
                            input[x, y + 1] = false;
                            input[x + 1, y] = false;
                            input[x + 1, y + 1] = false;
                            any = true;
                        }
            }
        }
        public static void Cleanup(BooleanMatrix binary, BooleanMatrix mask)
        {
            var size = binary.Size;
            var inverted = new BooleanMatrix(binary);
            inverted.Invert();
            var islands = VoteFilter.Vote(inverted, mask, Parameters.BinarizedVoteRadius, Parameters.BinarizedVoteMajority, Parameters.BinarizedVoteBorderDistance);
            var holes = VoteFilter.Vote(binary, mask, Parameters.BinarizedVoteRadius, Parameters.BinarizedVoteMajority, Parameters.BinarizedVoteBorderDistance);
            for (int y = 0; y < size.Y; ++y)
                for (int x = 0; x < size.X; ++x)
                    binary[x, y] = binary[x, y] && !islands[x, y] || holes[x, y];
            RemoveCrosses(binary);
            // https://sourceafis.machinezoo.com/transparency/filtered-binary-image
            FingerprintTransparency.Current.Log("filtered-binary-image", binary);
        }
        public static BooleanMatrix Invert(BooleanMatrix binary, BooleanMatrix mask)
        {
            var size = binary.Size;
            var inverted = new BooleanMatrix(size);
            for (int y = 0; y < size.Y; ++y)
                for (int x = 0; x < size.X; ++x)
                    inverted[x, y] = !binary[x, y] && mask[x, y];
            return inverted;
        }
    }
}
