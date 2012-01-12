using System;

namespace SourceAFIS.General
{
    public struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator PointF(Point point)
        {
            return new PointF(point.X, point.Y);
        }

        public static PointF operator +(PointF left, SizeF right)
        {
            return new PointF(left.X + right.Width, left.Y + right.Height);
        }

        public static PointF operator *(float factor, PointF point)
        {
            return new PointF(factor * point.X, factor * point.Y);
        }

        public static implicit operator System.Windows.Point(PointF point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }
    }
}
