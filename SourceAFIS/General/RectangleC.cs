using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    // coordinates in mathematically correct Cartesian plane, Y axis points upwards
    public struct RectangleC : IList<Point>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Left { get { return X; } set { Width += X - value; X = value; } }
        public int Bottom { get { return Y; } set { Height += Y - value;  Y = value; } }
        public int Right { get { return X + Width; } set { Width = value - X; } }
        public int Top { get { return Y + Height; } set { Height = value - Y; } }

        public Point Point { get { return new Point(Left, Bottom); } set { X = value.X; Y = value.Y; } }
        public Size Size { get { return new Size(Width, Height); } set { Width = value.Width; Height = value.Height; } }
        
        public Range RangeX { get { return new Range(Left, Right); } }
        public Range RangeY { get { return new Range(Bottom, Top); } }

        public Point Center { get { return new Point((Right + Left) / 2, (Bottom + Top) / 2); } }
        public int TotalArea { get { return Width * Height; } }

        public RectangleC(RectangleC other)
        {
            X = other.X;
            Y = other.Y;
            Width = other.Width;
            Height = other.Height;
        }

        public RectangleC(Point at, Size size)
        {
            X = at.X;
            Y = at.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleC(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleC(Point begin, Point end)
        {
            X = begin.X;
            Y = begin.Y;
            Width = end.X - begin.X;
            Height = end.Y - begin.Y;
        }

        public RectangleC(Size size)
        {
            X = 0;
            Y = 0;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleC(int width, int height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
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
            return new PointF(relative.X / (double)Width, relative.Y / (double)Height);
        }

        public void Shift(Point relative)
        {
            Point = MathEx.Add(Point, relative);
        }

        public RectangleC GetShifted(Point relative)
        {
            RectangleC result = new RectangleC(this);
            result.Shift(relative);
            return result;
        }

        public void Clip(RectangleC other)
        {
            if (Left < other.Left)
                Left = other.Left;
            if (Right > other.Right)
                Right = other.Right;
            if (Bottom < other.Bottom)
                Bottom = other.Bottom;
            if (Top > other.Top)
                Top = other.Top;
        }

        public void Include(Point point)
        {
            if (Left > point.X)
                Left = point.X;
            if (Right <= point.X)
                Right = point.X + 1;
            if (Bottom > point.Y)
                Bottom = point.Y;
            if (Top <= point.Y)
                Top = point.Y + 1;
        }

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator()
        {
            Point point = new Point();
            for (point.Y = Bottom; point.Y < Top; ++point.Y)
                for (point.X = Left; point.X < Right; ++point.X)
                    yield return point;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Point>)this).GetEnumerator();
        }

        int IList<Point>.IndexOf(Point point) { throw new NotImplementedException(); }
        void IList<Point>.Insert(int at, Point point) { throw new NotSupportedException(); }
        void IList<Point>.RemoveAt(int at) { throw new NotSupportedException(); }
        void ICollection<Point>.Add(Point point) { throw new NotSupportedException(); }
        bool ICollection<Point>.Remove(Point point) { throw new NotSupportedException(); }
        void ICollection<Point>.Clear() { throw new NotSupportedException(); }
        void ICollection<Point>.CopyTo(Point[] array, int count) { throw new NotImplementedException(); }
        bool ICollection<Point>.IsReadOnly { get { return true; } }
        int ICollection<Point>.Count { get { return TotalArea; } }
        Point IList<Point>.this[int at] { get { return new Point(at % Width, at / Width); } set { throw new NotSupportedException(); } }
    }
}
