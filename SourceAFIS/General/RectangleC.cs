using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    // coordinates in mathematically correct Cartesian plane, Y axis points upwards
    public struct RectangleC
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Left { get { return X; } set { X = value; } }
        public int Bottom { get { return Y; } set { Y = value; } }
        public int Right { get { return X + Width; } set { Width = value - X; } }
        public int Top { get { return Y + Height; } set { Height = value - Y; } }

        public Point Center { get { return new Point((Right + Left) / 2, (Bottom + Top) / 2); } }
        public int TotalArea { get { return Width * Height; } }

        public RectangleC(Point at, Size size)
        {
            X = at.X;
            Y = at.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public bool Contains(Point point)
        {
            return point.X >= Left && point.Y >= Bottom && point.X < Right && point.Y < Top;
        }

        public Point GetRelative(Point absolute)
        {
            return new Point(absolute.X - X, absolute.Y - Y);
        }

        public PointF GetFraction(Point absolute)
        {
            Point relative = GetRelative(absolute);
            return new PointF(relative.X / (float)Width, relative.Y / (float)Height);
        }
    }
}
