using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class EdgeAnalysis
    {
        public Template Template;
        public int ReferenceIndex;
        public int NeighborIndex;

        public float EdgeLength;
        public byte EdgeAngle;
        public byte ReferenceAngle;
        public byte NeighborAngle;

        public void ComputeAll()
        {
            ComputeLength();
            ComputeEdgeAngle();
            ComputeReferenceAngle();
            ComputeNeighborAngle();
        }

        public void ComputeLength()
        {
            EdgeLength = (float)Math.Sqrt(Calc.DistanceSq(Template.Minutiae[ReferenceIndex].Position, Template.Minutiae[NeighborIndex].Position));
        }

        public void ComputeEdgeAngle()
        {
            EdgeAngle = Angle.AtanB(Template.Minutiae[ReferenceIndex].Position, Template.Minutiae[NeighborIndex].Position);
        }

        public void ComputeReferenceAngle()
        {
            ReferenceAngle = Angle.Difference(Template.Minutiae[ReferenceIndex].Direction, EdgeAngle);
        }

        public void ComputeNeighborAngle()
        {
            NeighborAngle = Angle.Difference(Template.Minutiae[NeighborIndex].Direction, Angle.Opposite(EdgeAngle));
        }
    }
}
