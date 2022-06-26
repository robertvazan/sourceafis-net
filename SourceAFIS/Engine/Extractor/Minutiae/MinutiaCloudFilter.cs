// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor.Minutiae
{
    static class MinutiaCloudFilter
    {
        public static void Apply(List<Minutia> minutiae)
        {
            var radiusSq = Integers.Sq(Parameters.MinutiaCloudRadius);
            var kept = minutiae.Where(m => Parameters.MaxCloudSize >= minutiae.Where(n => (n.Position - m.Position).LengthSq <= radiusSq).Count() - 1).ToList();
            minutiae.Clear();
            minutiae.AddRange(kept);
        }
    }
}
