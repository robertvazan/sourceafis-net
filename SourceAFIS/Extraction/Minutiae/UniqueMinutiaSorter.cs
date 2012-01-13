using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class UniqueMinutiaSorter
    {
        [Parameter(Lower = 25, Upper = 1000)]
        public int MaxMinutiae = 100;
        [Parameter(Upper = 20)]
        public int NeighborhoodSize = 5;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Filter(TemplateBuilder template)
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
            Logger.Log(template);
        }
    }
}
