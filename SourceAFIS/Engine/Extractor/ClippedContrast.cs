// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor
{
    static class ClippedContrast
    {
        public static DoubleMatrix Compute(BlockMap blocks, HistogramCube histogram)
        {
            var result = new DoubleMatrix(blocks.Primary.Blocks);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                int volume = histogram.Sum(block);
                int clipLimit = Doubles.RoundToInt(volume * Parameters.ClippedContrast);
                int accumulator = 0;
                int lowerBound = histogram.Bins - 1;
                for (int i = 0; i < histogram.Bins; ++i)
                {
                    accumulator += histogram[block, i];
                    if (accumulator > clipLimit)
                    {
                        lowerBound = i;
                        break;
                    }
                }
                accumulator = 0;
                int upperBound = 0;
                for (int i = histogram.Bins - 1; i >= 0; --i)
                {
                    accumulator += histogram[block, i];
                    if (accumulator > clipLimit)
                    {
                        upperBound = i;
                        break;
                    }
                }
                result[block] = (upperBound - lowerBound) * (1.0 / (histogram.Bins - 1));
            }
            // https://sourceafis.machinezoo.com/transparency/contrast
            FingerprintTransparency.Current.Log("contrast", result);
            return result;
        }
    }
}
