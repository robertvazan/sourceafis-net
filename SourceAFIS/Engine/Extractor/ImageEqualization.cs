// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor
{
    static class ImageEqualization
    {
        public static DoubleMatrix Equalize(BlockMap blocks, DoubleMatrix image, HistogramCube histogram, BooleanMatrix blockMask)
        {
            const double rangeMin = -1;
            const double rangeMax = 1;
            const double rangeSize = rangeMax - rangeMin;
            double widthMax = rangeSize / histogram.Bins * Parameters.MaxEqualizationScaling;
            double widthMin = rangeSize / histogram.Bins * Parameters.MinEqualizationScaling;
            var limitedMin = new double[histogram.Bins];
            var limitedMax = new double[histogram.Bins];
            var dequantized = new double[histogram.Bins];
            for (int i = 0; i < histogram.Bins; ++i)
            {
                limitedMin[i] = Math.Max(i * widthMin + rangeMin, rangeMax - (histogram.Bins - 1 - i) * widthMax);
                limitedMax[i] = Math.Min(i * widthMax + rangeMin, rangeMax - (histogram.Bins - 1 - i) * widthMin);
                dequantized[i] = i / (double)(histogram.Bins - 1);
            }
            var mappings = new Dictionary<IntPoint, double[]>();
            foreach (var corner in blocks.Secondary.Blocks.Iterate())
            {
                double[] mapping = new double[histogram.Bins];
                mappings[corner] = mapping;
                if (blockMask.Get(corner, false)
                    || blockMask.Get(corner.X - 1, corner.Y, false)
                    || blockMask.Get(corner.X, corner.Y - 1, false)
                    || blockMask.Get(corner.X - 1, corner.Y - 1, false))
                {
                    double step = rangeSize / histogram.Sum(corner);
                    double top = rangeMin;
                    for (int i = 0; i < histogram.Bins; ++i)
                    {
                        double band = histogram[corner, i] * step;
                        double equalized = top + dequantized[i] * band;
                        top += band;
                        if (equalized < limitedMin[i])
                            equalized = limitedMin[i];
                        if (equalized > limitedMax[i])
                            equalized = limitedMax[i];
                        mapping[i] = equalized;
                    }
                }
            }
            var result = new DoubleMatrix(blocks.Pixels);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                var area = blocks.Primary.Block(block);
                if (blockMask[block])
                {
                    var topleft = mappings[block];
                    var topright = mappings[new IntPoint(block.X + 1, block.Y)];
                    var bottomleft = mappings[new IntPoint(block.X, block.Y + 1)];
                    var bottomright = mappings[new IntPoint(block.X + 1, block.Y + 1)];
                    for (int y = area.Top; y < area.Bottom; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                        {
                            int depth = histogram.Constrain((int)(image[x, y] * histogram.Bins));
                            double rx = (x - area.X + 0.5) / area.Width;
                            double ry = (y - area.Y + 0.5) / area.Height;
                            result[x, y] = Doubles.Interpolate(bottomleft[depth], bottomright[depth], topleft[depth], topright[depth], rx, ry);
                        }
                }
                else
                {
                    for (int y = area.Top; y < area.Bottom; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            result[x, y] = -1;
                }
            }
            // https://sourceafis.machinezoo.com/transparency/equalized-image
            FingerprintTransparency.Current.Log("equalized-image", result);
            return result;
        }
    }
}
