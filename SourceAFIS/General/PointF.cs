using System;

namespace SourceAFIS.General
{
    public struct PointF
    {
        public double X;
        public double Y;

        public PointF(double x, double y)
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

        public static PointF operator *(double factor, PointF point)
        {
            return new PointF(factor * point.X, factor * point.Y);
        }
    }
}
