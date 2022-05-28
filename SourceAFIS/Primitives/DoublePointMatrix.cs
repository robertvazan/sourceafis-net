// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    class DoublePointMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly double[] vectors;

        public IntPoint Size => new IntPoint(Width, Height);

        public DoublePointMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            vectors = new double[2 * width * height];
        }
        public DoublePointMatrix(IntPoint size) : this(size.X, size.Y) { }

        public DoublePoint this[int x, int y]
        {
            get
            {
                int i = Offset(x, y);
                return new DoublePoint(vectors[i], vectors[i + 1]);
            }
            set
            {
                int i = Offset(x, y);
                vectors[i] = value.X;
                vectors[i + 1] = value.Y;
            }
        }
        public DoublePoint this[IntPoint at]
        {
            get => this[at.X, at.Y];
            set => this[at.X, at.Y] = value;
        }

        public void Set(int x, int y, double px, double py)
        {
            int i = Offset(x, y);
            vectors[i] = px;
            vectors[i + 1] = py;
        }
        public void Add(int x, int y, double px, double py)
        {
            int i = Offset(x, y);
            vectors[i] += px;
            vectors[i + 1] += py;
        }
        public void Add(int x, int y, DoublePoint point) => Add(x, y, point.X, point.Y);
        public void Add(IntPoint at, DoublePoint point) => Add(at.X, at.Y, point);
        int Offset(int x, int y) => 2 * (y * Width + x);
    }
}
