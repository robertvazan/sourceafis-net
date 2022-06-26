// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Minutiae
{
    static class TopMinutiaeFilter
    {
        public static List<Minutia> Apply(List<Minutia> minutiae)
        {
            if (minutiae.Count <= Parameters.MaxMinutiae)
                return minutiae;
            return
                (from minutia in minutiae
                 let radiusSq = (from neighbor in minutiae
                                 let distanceSq = (minutia.Position - neighbor.Position).LengthSq
                                 orderby distanceSq
                                 select distanceSq).Skip(Parameters.SortByNeighbor).First()
                 orderby radiusSq descending
                 select minutia).Take(Parameters.MaxMinutiae).ToList();
        }
    }
}
