// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Primitives
{
    class BooleanMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly bool[] cells;

        public IntPoint Size => new IntPoint(Width, Height);

        public BooleanMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new bool[width * height];
        }
        public BooleanMatrix(IntPoint size) : this(size.X, size.Y) { }
        public BooleanMatrix(BooleanMatrix other)
            : this(other.Size)
        {
            for (int i = 0; i < cells.Length; ++i)
                cells[i] = other.cells[i];
        }

        public bool this[int x, int y]
        {
            get => cells[Offset(x, y)];
            set => cells[Offset(x, y)] = value;
        }
        public bool this[IntPoint at]
        {
            get => this[at.X, at.Y];
            set => this[at.X, at.Y] = value;
        }

        public bool Get(int x, int y, bool fallback)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return fallback;
            return cells[Offset(x, y)];
        }
        public bool Get(IntPoint at, bool fallback) => Get(at.X, at.Y, fallback);
        public void Invert()
        {
            for (int i = 0; i < cells.Length; ++i)
                cells[i] = !cells[i];
        }
        public void Merge(BooleanMatrix other)
        {
            if (other.Width != Width || other.Height != Height)
                throw new ArgumentException();
            for (int i = 0; i < cells.Length; ++i)
                cells[i] |= other.cells[i];
        }
        int Offset(int x, int y) => y * Width + x;
    }
}
