// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS.Primitives
{
    readonly struct IntRect : IEquatable<IntRect>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public int Left { get { return X; } }
        public int Top { get { return Y; } }
        public int Right { get { return X + Width; } }
        public int Bottom { get { return Y + Height; } }
        public int Area { get { return Width * Height; } }
        public IntPoint Center { get { return new IntPoint((Left + Right) / 2, (Top + Bottom) / 2); } }

        public IntRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public IntRect(IntPoint size)
        {
            X = 0;
            Y = 0;
            Width = size.X;
            Height = size.Y;
        }

        public static bool operator ==(IntRect left, IntRect right) => left.Equals(right);
        public static bool operator !=(IntRect left, IntRect right) => !left.Equals(right);

        public override int GetHashCode() { return ((X * 31 + Y) * 31 + Width) * 31 + Height; }
        public bool Equals(IntRect other) { return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height; }
        public override bool Equals(object other) { return other is IntRect && this == (IntRect)other; }
        public override string ToString() { return string.Format("{0}x{1} @ [{2},{3}]", Width, Height, X, Y); }
        public static IntRect Between(int startX, int startY, int endX, int endY) { return new IntRect(startX, startY, endX - startX, endY - startY); }
        public static IntRect Between(IntPoint start, IntPoint end) { return new IntRect(start.X, start.Y, end.X - start.X, end.Y - start.Y); }
        public static IntRect Around(int x, int y, int radius) { return Between(x - radius, y - radius, x + radius + 1, y + radius + 1); }
        public static IntRect Around(IntPoint center, int radius) { return Around(center.X, center.Y, radius); }
        public IntRect Intersect(IntRect other)
        {
            return Between(
                new IntPoint(Math.Max(Left, other.Left), Math.Max(Top, other.Top)),
                new IntPoint(Math.Min(Right, other.Right), Math.Min(Bottom, other.Bottom)));
        }
        public IntRect Move(IntPoint delta) { return new IntRect(X + delta.X, Y + delta.Y, Width, Height); }

        public IEnumerable<IntPoint> Iterate()
        {
            for (int y = Top; y < Bottom; ++y)
                for (int x = Left; x < Right; ++x)
                    yield return new IntPoint(x, y);
        }
    }
}
