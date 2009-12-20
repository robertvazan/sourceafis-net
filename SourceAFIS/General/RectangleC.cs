using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    // coordinates in mathematically correct Cartesian plane, Y axis points upwards
    public struct RectangleC : IList<Point>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Left { get { return X; } set { X = value; } }
        public int Bottom { get { return Y; } set { Y = value; } }
        public int Right { get { return X + Width; } set { Width = value - X; } }
        public int Top { get { return Y + Height; } set { Height = value - Y; } }

        public Point BottomLeft { get { return new Point(Left, Bottom); } set { Left = value.X; Bottom = value.Y; } }
        
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
            return new PointF(relative.X / (float)Width, relative.Y / (float)Height);
        }

        public void Shift(Point relative)
        {
            BottomLeft = Calc.Add(BottomLeft, relative);
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

        sealed class RectEnumerator : IEnumerator<Point>
        {
            RectangleC Rect;
            Point At;

            public RectEnumerator(RectangleC rect)
            {
                Rect = rect;
                At.X = rect.X - 1;
                At.Y = rect.Y;
            }

            public Point Current { get { return At; } }
            object IEnumerator.Current { get { return At; } }

            public bool MoveNext()
            {
                ++At.X;
                if (At.X >= Rect.Right)
                {
                    ++At.Y;
                    if (At.Y >= Rect.Top)
                        return false;
                    At.X = Rect.Left;
                }
                return true;
            }

            public void Reset()
            {
                At.X = Rect.X - 1;
                At.Y = Rect.Y;
            }

            public void Dispose()
            {
            }
        }

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator()
        {
            return new RectEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RectEnumerator(this);
        }

        int IList<Point>.IndexOf(Point point) { throw new Exception(); }
        void IList<Point>.Insert(int at, Point point) { throw new Exception(); }
        void IList<Point>.RemoveAt(int at) { throw new Exception(); }
        void ICollection<Point>.Add(Point point) { throw new Exception(); }
        bool ICollection<Point>.Remove(Point point) { throw new Exception(); }
        void ICollection<Point>.Clear() { throw new Exception(); }
        void ICollection<Point>.CopyTo(Point[] array, int count) { throw new Exception(); }
        bool ICollection<Point>.IsReadOnly { get { return true; } }
        int ICollection<Point>.Count { get { return TotalArea; } }
        Point IList<Point>.this[int at] { get { return new Point(at % Width, at / Width); } set { throw new Exception(); } }
    }
}
