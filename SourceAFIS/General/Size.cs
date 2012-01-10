using System;

namespace SourceAFIS.General
{
    public struct Size
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Size(Point point)
        {
            Width = point.X;
            Height = point.Y;
        }

        public override bool Equals(object other)
        {
            return other is Size && this == (Size)other;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public static bool operator ==(Size left, Size right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }
    }
}
