using System;

namespace SourceAFIS.Utils
{
    struct PointF
    {
        public double X;
        public double Y;

        public PointF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point Round()
        {
            return new Point(Convert.ToInt32(X), Convert.ToInt32(Y));
        }

        public static implicit operator PointF(Point point)
        {
            return new PointF(point.X, point.Y);
        }

        public static PointF operator +(PointF left, PointF right)
        {
            return new PointF(left.X + right.X, left.Y + right.Y);
        }

        public static PointF operator *(double factor, PointF point)
        {
            return new PointF(factor * point.X, factor * point.Y);
        }
    }
}
