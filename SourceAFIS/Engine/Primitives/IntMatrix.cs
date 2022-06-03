// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Primitives
{
    class IntMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly int[] cells;

        public IntPoint Size => new IntPoint(Width, Height);

        public IntMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new int[width * height];
        }
        public IntMatrix(IntPoint size) : this(size.X, size.Y) { }

        public int this[int x, int y]
        {
            get => cells[Offset(x, y)];
            set => cells[Offset(x, y)] = value;
        }
        public int this[IntPoint at]
        {
            get => this[at.X, at.Y];
            set => this[at.X, at.Y] = value;
        }

        int Offset(int x, int y) => y * Width + x;
    }
}
