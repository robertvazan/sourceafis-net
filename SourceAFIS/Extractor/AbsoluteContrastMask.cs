// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Configuration;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor
{
    static class AbsoluteContrastMask
    {
        public static BooleanMatrix Compute(DoubleMatrix contrast)
        {
            var result = new BooleanMatrix(contrast.Size);
            foreach (var block in contrast.Size.Iterate())
                if (contrast[block] < Parameters.MinAbsoluteContrast)
                    result[block] = true;
            // https://sourceafis.machinezoo.com/transparency/absolute-contrast-mask
            FingerprintTransparency.Current.Log("absolute-contrast-mask", result);
            return result;
        }
    }
}
