// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Engine.Primitives
{
    readonly struct ShortPoint : IEquatable<ShortPoint>, IComparable<ShortPoint>
    {
        public readonly short X;
        public readonly short Y;

        public int LengthSq => Integers.Sq(X) + Integers.Sq(Y);

        public ShortPoint(short x, short y)
        {
            X = x;
            Y = y;
        }
        public ShortPoint(int x, int y)
        {
            X = (short)x;
            Y = (short)y;
        }

        public static ShortPoint operator +(ShortPoint left, ShortPoint right) => new(left.X + right.X, left.Y + right.Y);
        public static ShortPoint operator -(ShortPoint left, ShortPoint right) => new(left.X - right.X, left.Y - right.Y);
        public static ShortPoint operator -(ShortPoint point) => new(-point.X, -point.Y);
        public static bool operator ==(ShortPoint left, ShortPoint right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(ShortPoint left, ShortPoint right) => left.X != right.X || left.Y != right.Y;

        public override int GetHashCode() => 31 * X + Y;
        public bool Equals(ShortPoint other) => X == other.X && Y == other.Y;
        public override bool Equals(object other) => other is ShortPoint p && Equals(p);
        public int CompareTo(ShortPoint other)
        {
            int resultY = Y.CompareTo(other.Y);
            if (resultY != 0)
                return resultY;
            return X.CompareTo(other.X);
        }
        public override string ToString() => $"[{X},{Y}]";
        public IntPoint ToInt() => new(X, Y);
    }
}
