// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Primitives
{
    class DoubleMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly double[] cells;

        public IntPoint Size => new IntPoint(Width, Height);

        public DoubleMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new double[width * height];
        }
        public DoubleMatrix(IntPoint size) : this(size.X, size.Y) { }

        public double this[int x, int y]
        {
            get => cells[Offset(x, y)];
            set => cells[Offset(x, y)] = value;
        }
        public double this[IntPoint at]
        {
            get => this[at.X, at.Y];
            set => this[at.X, at.Y] = value;
        }

        public void Add(int x, int y, double value) => cells[Offset(x, y)] += value;
        public void Add(IntPoint at, double value) => Add(at.X, at.Y, value);
        public void Multiply(int x, int y, double value) => cells[Offset(x, y)] *= value;
        public void Multiply(IntPoint at, double value) => Multiply(at.X, at.Y, value);
        int Offset(int x, int y) => y * Width + x;
    }
}
