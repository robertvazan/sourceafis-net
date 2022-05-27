// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Configuration;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor
{
    static class PixelwiseOrientations
    {
        class ConsideredOrientation
        {
            public IntPoint Offset;
            public DoublePoint Orientation;
        }
        class OrientationRandom
        {
            const int Prime = 1610612741;
            const int Bits = 30;
            const int Mask = (1 << Bits) - 1;
            const double Scaling = 1.0 / (1 << Bits);
            long state = unchecked(Prime * Prime * Prime);
            public double Next()
            {
                state *= Prime;
                return ((state & Mask) + 0.5) * Scaling;
            }
        }
        static ConsideredOrientation[][] Plan()
        {
            var random = new OrientationRandom();
            var splits = new ConsideredOrientation[Parameters.OrientationSplit][];
            for (int i = 0; i < Parameters.OrientationSplit; ++i)
            {
                var orientations = splits[i] = new ConsideredOrientation[Parameters.OrientationsChecked];
                for (int j = 0; j < Parameters.OrientationsChecked; ++j)
                {
                    var sample = orientations[j] = new ConsideredOrientation();
                    while (true)
                    {
                        double angle = random.Next() * Math.PI;
                        double distance = Doubles.InterpolateExponential(Parameters.MinOrientationRadius, Parameters.MaxOrientationRadius, random.Next());
                        sample.Offset = (distance * DoubleAngle.ToVector(angle)).Round();
                        if (sample.Offset == IntPoint.Zero)
                            continue;
                        if (sample.Offset.Y < 0)
                            continue;
                        bool duplicate = false;
                        for (int jj = 0; jj < j; ++jj)
                            if (orientations[jj].Offset == sample.Offset)
                                duplicate = true;
                        if (duplicate)
                            continue;
                        break;
                    }
                    sample.Orientation = DoubleAngle.ToVector(DoubleAngle.Add(DoubleAngle.ToOrientation(DoubleAngle.Atan((DoublePoint)sample.Offset)), Math.PI));
                }
            }
            return splits;
        }
        static IntRange MaskRange(BooleanMatrix mask, int y)
        {
            int first = -1;
            int last = -1;
            for (int x = 0; x < mask.Width; ++x)
                if (mask[x, y])
                {
                    last = x;
                    if (first < 0)
                        first = x;
                }
            if (first >= 0)
                return new IntRange(first, last + 1);
            else
                return IntRange.Zero;
        }
        public static DoublePointMatrix Compute(DoubleMatrix input, BooleanMatrix mask, BlockMap blocks)
        {
            var neighbors = Plan();
            var orientation = new DoublePointMatrix(input.Size);
            for (int blockY = 0; blockY < blocks.Primary.Blocks.Y; ++blockY)
            {
                var maskRange = MaskRange(mask, blockY);
                if (maskRange.Length > 0)
                {
                    var validXRange = new IntRange(
                        blocks.Primary.Block(maskRange.Start, blockY).Left,
                        blocks.Primary.Block(maskRange.End - 1, blockY).Right);
                    for (int y = blocks.Primary.Block(0, blockY).Top; y < blocks.Primary.Block(0, blockY).Bottom; ++y)
                    {
                        foreach (var neighbor in neighbors[y % neighbors.Length])
                        {
                            int radius = Math.Max(Math.Abs(neighbor.Offset.X), Math.Abs(neighbor.Offset.Y));
                            if (y - radius >= 0 && y + radius < input.Height)
                            {
                                var xRange = new IntRange(Math.Max(radius, validXRange.Start), Math.Min(input.Width - radius, validXRange.End));
                                for (int x = xRange.Start; x < xRange.End; ++x)
                                {
                                    double before = input[x - neighbor.Offset.X, y - neighbor.Offset.Y];
                                    double at = input[x, y];
                                    double after = input[x + neighbor.Offset.X, y + neighbor.Offset.Y];
                                    double strength = at - Math.Max(before, after);
                                    if (strength > 0)
                                        orientation.Add(x, y, strength * neighbor.Orientation);
                                }
                            }
                        }
                    }
                }
            }
            // https://sourceafis.machinezoo.com/transparency/pixelwise-orientation
            FingerprintTransparency.Current.Log("pixelwise-orientation", orientation);
            return orientation;
        }
    }
}
