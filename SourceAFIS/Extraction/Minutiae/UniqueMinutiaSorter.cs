using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class UniqueMinutiaSorter
    {
        const int MaxMinutiae = 100;
        const int NeighborhoodSize = 5;

        public void Filter(FingerprintTemplate template)
        {
            if (template.Minutiae.Count > MaxMinutiae)
            {
                template.Minutiae =
                    (from minutia in template.Minutiae
                     let radiusSq = (from neighbor in template.Minutiae
                                     let distanceSq = Calc.DistanceSq(minutia.Position, neighbor.Position)
                                     orderby distanceSq
                                     select distanceSq).Skip(NeighborhoodSize).First()
                     orderby radiusSq descending
                     select minutia).Take(MaxMinutiae).ToList();
            }
        }
    }
}
