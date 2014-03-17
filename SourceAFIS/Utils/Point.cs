using System;
using System.Collections.Generic;
using System.Collections;

namespace SourceAFIS.Utils
{
    struct Point : IEnumerable<Point>
    {
        public int X;
        public int Y;
        public int Area { get { return X * Y; } }
        public int SqLength { get { return MathEx.Sq(X) + MathEx.Sq(Y); } }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode() { return X.GetHashCode() + Y.GetHashCode(); }

        public override bool Equals(object other) { return other is Point && this == (Point)other; }

        public static Point SizeOf<T>(T[,] array) { return new Point(array.GetLength(1), array.GetLength(0)); }

        public bool Contains(Point inner) { return inner.X >= 0 && inner.Y >= 0 && inner.X < X && inner.Y < Y; }

        public T[,] Allocate<T>() { return new T[Y, X]; }
        public T Get<T>(T[,] array) { return array[Y, X]; }
        public T Get<T>(T[,] array, T fallback) { return X >= 0 && Y >= 0 && X < array.GetLength(1) && Y < array.GetLength(0) ? array[Y, X] : fallback; }
        public void Set<T>(T[,] array, T value) { array[Y, X] = value; }

        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Point left, Point right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point operator -(Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator()
        {
            Point point = new Point();
            for (point.Y = 0; point.Y < Y; ++point.Y)
                for (point.X = 0; point.X < X; ++point.X)
                    yield return point;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Point>)this).GetEnumerator();
        }
    }
}
