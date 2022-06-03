// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Minutiae
{
    static class MinutiaCollector
    {
        static void Collect(List<MutableMinutia> minutiae, Skeleton skeleton, MinutiaType type)
        {
            foreach (var sminutia in skeleton.Minutiae)
                if (sminutia.Ridges.Count == 1)
                    minutiae.Add(new MutableMinutia(sminutia.Position, sminutia.Ridges[0].Direction(), type));
        }
        public static List<MutableMinutia> Collect(Skeleton ridges, Skeleton valleys)
        {
            var minutiae = new List<MutableMinutia>();
            Collect(minutiae, ridges, MinutiaType.Ending);
            Collect(minutiae, valleys, MinutiaType.Bifurcation);
            return minutiae;
        }
    }
}
