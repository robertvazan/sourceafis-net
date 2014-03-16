using System;

namespace SourceAFIS.General
{
    public struct SizeF
    {
        public double Width;
        public double Height;

        public SizeF(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public SizeF(PointF point)
        {
            Width = point.X;
            Height = point.Y;
        }
    }
}
