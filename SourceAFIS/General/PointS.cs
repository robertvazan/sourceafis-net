using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    [Serializable]
    public struct PointS
    {
        public int X;
        public int Y;

        public PointS(Point point)
        {
            X = (int)point.X;
            Y = (int)point.Y;
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
