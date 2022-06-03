// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS.Engine.Primitives
{
    readonly struct IntRect : IEquatable<IntRect>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public int Area => Width * Height;
        public IntPoint Center => new IntPoint((Left + Right) / 2, (Top + Bottom) / 2);

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

        public override int GetHashCode() => ((X * 31 + Y) * 31 + Width) * 31 + Height;
        public bool Equals(IntRect other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        public override bool Equals(object other) => other is IntRect r && Equals(r);
        public override string ToString() => $"{Width}x{Height} @ [{X},{Y}]";
        public static IntRect Between(int startX, int startY, int endX, int endY) => new IntRect(startX, startY, endX - startX, endY - startY);
        public static IntRect Between(IntPoint start, IntPoint end) => new IntRect(start.X, start.Y, end.X - start.X, end.Y - start.Y);
        public static IntRect Around(int x, int y, int radius) => Between(x - radius, y - radius, x + radius + 1, y + radius + 1);
        public static IntRect Around(IntPoint center, int radius) => Around(center.X, center.Y, radius);
        public IntRect Intersect(IntRect other)
        {
            return Between(
                new IntPoint(Math.Max(Left, other.Left), Math.Max(Top, other.Top)),
                new IntPoint(Math.Min(Right, other.Right), Math.Min(Bottom, other.Bottom)));
        }
        public IntRect Move(IntPoint delta) => new IntRect(X + delta.X, Y + delta.Y, Width, Height);

        public IEnumerable<IntPoint> Iterate()
        {
            for (int y = Top; y < Bottom; ++y)
                for (int x = Left; x < Right; ++x)
                    yield return new IntPoint(x, y);
        }
    }
}
