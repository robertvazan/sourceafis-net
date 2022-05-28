// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS.Primitives
{
    readonly struct IntPoint : IEquatable<IntPoint>, IComparable<IntPoint>
    {
        public static readonly IntPoint Zero = new IntPoint();
        public static readonly IntPoint[] EdgeNeighbors = new IntPoint[] {
            new IntPoint(0, -1),
            new IntPoint(-1, 0),
            new IntPoint(1, 0),
            new IntPoint(0, 1)
        };
        public static readonly IntPoint[] CornerNeighbors = new IntPoint[] {
            new IntPoint(-1, -1),
            new IntPoint(0, -1),
            new IntPoint(1, -1),
            new IntPoint(-1, 0),
            new IntPoint(1, 0),
            new IntPoint(-1, 1),
            new IntPoint(0, 1),
            new IntPoint(1, 1)
        };

        public readonly int X;
        public readonly int Y;

        public int Area => X * Y;
        public int LengthSq => Integers.Sq(X) + Integers.Sq(Y);

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntPoint operator +(IntPoint left, IntPoint right) => new IntPoint(left.X + right.X, left.Y + right.Y);
        public static IntPoint operator -(IntPoint left, IntPoint right) => new IntPoint(left.X - right.X, left.Y - right.Y);
        public static IntPoint operator -(IntPoint point) => new IntPoint(-point.X, -point.Y);
        public static bool operator ==(IntPoint left, IntPoint right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(IntPoint left, IntPoint right) => left.X != right.X || left.Y != right.Y;

        public override int GetHashCode() => 31 * X + Y;
        public bool Equals(IntPoint other) => X == other.X && Y == other.Y;
        public override bool Equals(object other) => other is IntPoint p && Equals(p);
        public int CompareTo(IntPoint other)
        {
            int resultY = Y.CompareTo(other.Y);
            if (resultY != 0)
                return resultY;
            return X.CompareTo(other.X);
        }
        public override string ToString() => $"[{X},{Y}]";
        public bool Contains(IntPoint other) => other.X >= 0 && other.Y >= 0 && other.X < X && other.Y < Y;
        public IntPoint[] LineTo(IntPoint to)
        {
            IntPoint[] result;
            var relative = to - this;
            if (Math.Abs(relative.X) >= Math.Abs(relative.Y))
            {
                result = new IntPoint[Math.Abs(relative.X) + 1];
                if (relative.X > 0)
                {
                    for (int i = 0; i <= relative.X; ++i)
                        result[i] = new IntPoint(X + i, Y + Doubles.RoundToInt(i * (relative.Y / (double)relative.X)));
                }
                else if (relative.X < 0)
                {
                    for (int i = 0; i <= -relative.X; ++i)
                        result[i] = new IntPoint(X - i, Y - Doubles.RoundToInt(i * (relative.Y / (double)relative.X)));
                }
                else
                    result[0] = this;
            }
            else
            {
                result = new IntPoint[Math.Abs(relative.Y) + 1];
                if (relative.Y > 0)
                {
                    for (int i = 0; i <= relative.Y; ++i)
                        result[i] = new IntPoint(X + Doubles.RoundToInt(i * (relative.X / (double)relative.Y)), Y + i);
                }
                else if (relative.Y < 0)
                {
                    for (int i = 0; i <= -relative.Y; ++i)
                        result[i] = new IntPoint(X - Doubles.RoundToInt(i * (relative.X / (double)relative.Y)), Y - i);
                }
                else
                    result[0] = this;
            }
            return result;
        }

        public IEnumerable<IntPoint> Iterate()
        {
            for (int y = 0; y < Y; ++y)
                for (int x = 0; x < X; ++x)
                    yield return new IntPoint(x, y);
        }
    }
}
