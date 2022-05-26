// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    class IntMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly int[] Cells;

        public IntPoint Size { get { return new IntPoint(Width, Height); } }

        public IntMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new int[width * height];
        }
        public IntMatrix(IntPoint size) : this(size.X, size.Y) { }

        public int this[int x, int y]
        {
            get { return Cells[Offset(x, y)]; }
            set { Cells[Offset(x, y)] = value; }
        }
        public int this[IntPoint at]
        {
            get { return this[at.X, at.Y]; }
            set { this[at.X, at.Y] = value; }
        }

        int Offset(int x, int y) { return y * Width + x; }
    }
}
