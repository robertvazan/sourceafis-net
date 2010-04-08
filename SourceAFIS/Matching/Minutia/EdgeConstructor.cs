using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class EdgeConstructor
    {
        public EdgeInfo Construct(Template template, int reference, int neighbor)
        {
            PolarPoint polar = Angle.ToPolar(Calc.Difference(template.Minutiae[neighbor].Position, template.Minutiae[reference].Position));
            EdgeInfo edge;
            edge.Length = (short)polar.Distance;
            edge.ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, polar.Angle);
            edge.NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(polar.Angle));
            return edge;
        }
    }
}
