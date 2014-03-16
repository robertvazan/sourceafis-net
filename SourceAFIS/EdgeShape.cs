using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Utils;

namespace SourceAFIS
{
    struct EdgeShape
    {
        public readonly int Length;
        public readonly byte ReferenceAngle;
        public readonly byte NeighborAngle;

        public EdgeShape(FingerprintTemplate template, int reference, int neighbor)
        {
            var polar = Angle.ToPolar(template.Minutiae[neighbor].Position - template.Minutiae[reference].Position);
            Length = polar.Distance;
            ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, polar.Angle);
            NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(polar.Angle));
        }
    }
}
