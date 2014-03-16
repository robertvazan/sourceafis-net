using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public static class EdgeConstructor
    {
        public static EdgeShape Construct(FingerprintTemplate template, int reference, int neighbor)
        {
            PolarPoint polar = Angle.ToPolar(template.Minutiae[neighbor].Position - template.Minutiae[reference].Position);
            EdgeShape edge;
            edge.Length = polar.Distance;
            edge.ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, polar.Angle);
            edge.NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(polar.Angle));
            return edge;
        }
    }
}
