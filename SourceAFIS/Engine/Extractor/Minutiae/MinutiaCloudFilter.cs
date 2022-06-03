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
        public static void Apply(List<MutableMinutia> minutiae)
        {
            var radiusSq = Integers.Sq(Parameters.MinutiaCloudRadius);
            var removed = new HashSet<MutableMinutia>(minutiae.Where(minutia => Parameters.MaxCloudSize < minutiae.Where(neighbor => (neighbor.Position - minutia.Position).LengthSq <= radiusSq).Count() - 1));
            minutiae.RemoveAll(minutia => removed.Contains(minutia));
        }
    }
}
