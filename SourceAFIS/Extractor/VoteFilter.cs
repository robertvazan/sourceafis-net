// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor
{
    static class VoteFilter
    {
        public static BooleanMatrix Vote(BooleanMatrix input, BooleanMatrix mask, int radius, double majority, int borderDistance)
        {
            var size = input.Size;
            var rect = new IntRect(borderDistance, borderDistance, size.X - 2 * borderDistance, size.Y - 2 * borderDistance);
            int[] thresholds = new int[Integers.Sq(2 * radius + 1) + 1];
            for (int i = 0; i < thresholds.Length; ++i)
                thresholds[i] = (int)Math.Ceiling(majority * i);
            var counts = new IntMatrix(size);
            var output = new BooleanMatrix(size);
            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                int superTop = y - radius - 1;
                int superBottom = y + radius;
                int yMin = Math.Max(0, y - radius);
                int yMax = Math.Min(size.Y - 1, y + radius);
                int yRange = yMax - yMin + 1;
                for (int x = rect.Left; x < rect.Right; ++x)
                    if (mask == null || mask[x, y])
                    {
                        int left = x > 0 ? counts[x - 1, y] : 0;
                        int top = y > 0 ? counts[x, y - 1] : 0;
                        int diagonal = x > 0 && y > 0 ? counts[x - 1, y - 1] : 0;
                        int xMin = Math.Max(0, x - radius);
                        int xMax = Math.Min(size.X - 1, x + radius);
                        int ones;
                        if (left > 0 && top > 0 && diagonal > 0)
                        {
                            ones = top + left - diagonal - 1;
                            int superLeft = x - radius - 1;
                            int superRight = x + radius;
                            if (superLeft >= 0 && superTop >= 0 && input[superLeft, superTop])
                                ++ones;
                            if (superLeft >= 0 && superBottom < size.Y && input[superLeft, superBottom])
                                --ones;
                            if (superRight < size.X && superTop >= 0 && input[superRight, superTop])
                                --ones;
                            if (superRight < size.X && superBottom < size.Y && input[superRight, superBottom])
                                ++ones;
                        }
                        else
                        {
                            ones = 0;
                            for (int ny = yMin; ny <= yMax; ++ny)
                                for (int nx = xMin; nx <= xMax; ++nx)
                                    if (input[nx, ny])
                                        ++ones;
                        }
                        counts[x, y] = ones + 1;
                        if (ones >= thresholds[yRange * (xMax - xMin + 1)])
                            output[x, y] = true;
                    }
            }
            return output;
        }
    }
}
