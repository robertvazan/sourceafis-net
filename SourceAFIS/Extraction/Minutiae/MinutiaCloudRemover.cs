using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaCloudRemover
    {
        [Parameter(Upper = 300)]
        public int NeighborhoodRadius = 20;
        [Parameter(Upper = 30)]
        public int MaxNeighbors = 4;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Filter(FingerprintTemplate template)
        {
            var radiusSq = Calc.Sq(NeighborhoodRadius);
            template.Minutiae = template.Minutiae.Except(
                (from minutia in template.Minutiae
                 where template.Minutiae.Count(neighbor => Calc.DistanceSq(neighbor.Position, minutia.Position) <= radiusSq) - 1 > MaxNeighbors
                 select minutia).ToList()).ToList();
            Logger.Log(template);
        }
    }
}
