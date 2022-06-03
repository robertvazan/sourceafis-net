// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor
{
    static class OrientedSmoothing
    {
        static IntPoint[][] Lines(int resolution, int radius, double step)
        {
            var result = new IntPoint[resolution][];
            for (int orientationIndex = 0; orientationIndex < resolution; ++orientationIndex)
            {
                var line = new List<IntPoint>();
                line.Add(IntPoint.Zero);
                var direction = DoubleAngle.ToVector(DoubleAngle.FromOrientation(DoubleAngle.BucketCenter(orientationIndex, resolution)));
                for (double r = radius; r >= 0.5; r /= step)
                {
                    var sample = (r * direction).Round();
                    if (!line.Contains(sample))
                    {
                        line.Add(sample);
                        line.Add(-sample);
                    }
                }
                result[orientationIndex] = line.ToArray();
            }
            return result;
        }
        static DoubleMatrix Smooth(DoubleMatrix input, DoubleMatrix orientation, BooleanMatrix mask, BlockMap blocks, double angle, IntPoint[][] lines)
        {
            var output = new DoubleMatrix(input.Size);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                if (mask[block])
                {
                    var line = lines[DoubleAngle.Quantize(DoubleAngle.Add(orientation[block], angle), lines.Length)];
                    foreach (var linePoint in line)
                    {
                        var target = blocks.Primary.Block(block);
                        var source = target.Move(linePoint).Intersect(new IntRect(blocks.Pixels));
                        target = source.Move(-linePoint);
                        for (int y = target.Top; y < target.Bottom; ++y)
                            for (int x = target.Left; x < target.Right; ++x)
                                output.Add(x, y, input[x + linePoint.X, y + linePoint.Y]);
                    }
                    var blockArea = blocks.Primary.Block(block);
                    for (int y = blockArea.Top; y < blockArea.Bottom; ++y)
                        for (int x = blockArea.Left; x < blockArea.Right; ++x)
                            output.Multiply(x, y, 1.0 / line.Length);
                }
            }
            return output;
        }
        public static DoubleMatrix Parallel(DoubleMatrix input, DoubleMatrix orientation, BooleanMatrix mask, BlockMap blocks)
        {
            var lines = Lines(Parameters.ParallelSmoothingResolution, Parameters.ParallelSmoothingRadius, Parameters.ParallelSmoothingStep);
            var smoothed = Smooth(input, orientation, mask, blocks, 0, lines);
            // https://sourceafis.machinezoo.com/transparency/parallel-smoothing
            FingerprintTransparency.Current.Log("parallel-smoothing", smoothed);
            return smoothed;
        }
        public static DoubleMatrix Orthogonal(DoubleMatrix input, DoubleMatrix orientation, BooleanMatrix mask, BlockMap blocks)
        {
            var lines = Lines(Parameters.OrthogonalSmoothingResolution, Parameters.OrthogonalSmoothingRadius, Parameters.OrthogonalSmoothingStep);
            var smoothed = Smooth(input, orientation, mask, blocks, Math.PI, lines);
            // https://sourceafis.machinezoo.com/transparency/orthogonal-smoothing
            FingerprintTransparency.Current.Log("orthogonal-smoothing", smoothed);
            return smoothed;
        }
    }
}
