using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaCloudRemover
    {
        const int NeighborhoodRadius = 20;
        const int MaxNeighbors = 4;

        public void Filter(FingerprintTemplate template)
        {
            var radiusSq = Calc.Sq(NeighborhoodRadius);
            template.Minutiae = template.Minutiae.Except(
                (from minutia in template.Minutiae
                 where template.Minutiae.Count(neighbor => Calc.DistanceSq(neighbor.Position, minutia.Position) <= radiusSq) - 1 > MaxNeighbors
                 select minutia).ToList()).ToList();
        }
    }
}
