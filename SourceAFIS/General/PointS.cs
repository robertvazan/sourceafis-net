using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    [Serializable]
    public struct PointS
    {
        public short X;
        public short Y;

        public PointS(Point point)
        {
            X = (short)point.X;
            Y = (short)point.Y;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public static implicit operator Point(PointS point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
