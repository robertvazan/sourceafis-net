using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class EdgeConstructor
    {
        public EdgeInfo Construct(Template template, int reference, int neighbor)
        {
            EdgeInfo edge;
            edge.Length = (float)Math.Sqrt(Calc.DistanceSq(template.Minutiae[reference].Position, template.Minutiae[neighbor].Position));
            byte edgeAngle = Angle.AtanB(template.Minutiae[reference].Position, template.Minutiae[neighbor].Position);
            edge.ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, edgeAngle);
            edge.NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(edgeAngle));
            return edge;
        }
    }
}
