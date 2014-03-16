using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Utils
{
    struct PolarPoint
    {
        public int Distance;
        public byte Angle;

        public PolarPoint(int distance, byte angle)
        {
            Distance = distance;
            Angle = angle;
        }
    }
}
