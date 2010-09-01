using System;

namespace SourceAFIS.Dummy
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
    }
}
