// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Configuration;
using SourceAFIS.Features;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor.Minutiae
{
    static class InnerMinutiaeFilter
    {
        public static void Apply(List<MutableMinutia> minutiae, BooleanMatrix mask)
        {
            minutiae.RemoveAll(minutia =>
            {
                var arrow = (-Parameters.MaskDisplacement * DoubleAngle.ToVector(minutia.Direction)).Round();
                return !mask.Get(minutia.Position + arrow, false);
            });
        }
    }
}
