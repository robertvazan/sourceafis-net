// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor.Minutiae
{
    static class InnerMinutiaeFilter
    {
        public static void Apply(List<Minutia> minutiae, BooleanMatrix mask)
        {
            minutiae.RemoveAll(minutia =>
            {
                var arrow = (-Parameters.MaskDisplacement * DoubleAngle.ToVector(minutia.Direction)).Round();
                return !mask.Get(minutia.Position + arrow, false);
            });
        }
    }
}
