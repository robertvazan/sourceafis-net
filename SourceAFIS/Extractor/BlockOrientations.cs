// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Configuration;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor
{
    static class BlockOrientations
    {
        static DoublePointMatrix Aggregate(DoublePointMatrix orientation, BlockMap blocks, BooleanMatrix mask)
        {
            var sums = new DoublePointMatrix(blocks.Primary.Blocks);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                if (mask[block])
                {
                    var area = blocks.Primary.Block(block);
                    for (int y = area.Top; y < area.Bottom; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            sums.Add(block, orientation[x, y]);
                }
            }
            // https://sourceafis.machinezoo.com/transparency/block-orientation
            FingerprintTransparency.Current.Log("block-orientation", sums);
            return sums;
        }
        static DoublePointMatrix Smooth(DoublePointMatrix orientation, BooleanMatrix mask)
        {
            var size = mask.Size;
            var smoothed = new DoublePointMatrix(size);
            foreach (var block in size.Iterate())
                if (mask[block])
                {
                    var neighbors = IntRect.Around(block, Parameters.OrientationSmoothingRadius).Intersect(new IntRect(size));
                    for (int ny = neighbors.Top; ny < neighbors.Bottom; ++ny)
                        for (int nx = neighbors.Left; nx < neighbors.Right; ++nx)
                            if (mask[nx, ny])
                                smoothed.Add(block, orientation[nx, ny]);
                }
            // https://sourceafis.machinezoo.com/transparency/smoothed-orientation
            FingerprintTransparency.Current.Log("smoothed-orientation", smoothed);
            return smoothed;
        }
        static DoubleMatrix Angles(DoublePointMatrix vectors, BooleanMatrix mask)
        {
            var size = mask.Size;
            var angles = new DoubleMatrix(size);
            foreach (var block in size.Iterate())
                if (mask[block])
                    angles[block] = DoubleAngle.Atan(vectors[block]);
            return angles;
        }
        public static DoubleMatrix Compute(DoubleMatrix image, BooleanMatrix mask, BlockMap blocks)
        {
            var accumulated = PixelwiseOrientations.Compute(image, mask, blocks);
            var byBlock = Aggregate(accumulated, blocks, mask);
            var smooth = Smooth(byBlock, mask);
            return Angles(smooth, mask);
        }
    }
}
