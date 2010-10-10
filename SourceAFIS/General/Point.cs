using System;

namespace SourceAFIS.General
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode() { return X.GetHashCode() + Y.GetHashCode(); }

        public override bool Equals(object other) { return other is Point && this == (Point)other; }

        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Point left, Point right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static Point operator +(Point left, Size right)
        {
            return new Point(left.X + right.Width, left.Y + right.Height);
        }

        public static Point operator -(Point left, Size right)
        {
            return new Point(left.X - right.Width, left.Y - right.Height);
        }
    }
}
