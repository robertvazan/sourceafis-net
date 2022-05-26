// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    class DoublePointMatrix
    {
        public readonly int Width;
        public readonly int Height;
        readonly double[] Vectors;

        public IntPoint Size { get { return new IntPoint(Width, Height); } }

        public DoublePointMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            Vectors = new double[2 * width * height];
        }
        public DoublePointMatrix(IntPoint size) : this(size.X, size.Y) { }

        public DoublePoint this[int x, int y]
        {
            get
            {
                int i = Offset(x, y);
                return new DoublePoint(Vectors[i], Vectors[i + 1]);
            }
            set
            {
                int i = Offset(x, y);
                Vectors[i] = value.X;
                Vectors[i + 1] = value.Y;
            }
        }
        public DoublePoint this[IntPoint at]
        {
            get { return this[at.X, at.Y]; }
            set { this[at.X, at.Y] = value; }
        }

        public void Set(int x, int y, double px, double py)
        {
            int i = Offset(x, y);
            Vectors[i] = px;
            Vectors[i + 1] = py;
        }
        public void Add(int x, int y, double px, double py)
        {
            int i = Offset(x, y);
            Vectors[i] += px;
            Vectors[i + 1] += py;
        }
        public void Add(int x, int y, DoublePoint point) { Add(x, y, point.X, point.Y); }
        public void Add(IntPoint at, DoublePoint point) { Add(at.X, at.Y, point); }
        int Offset(int x, int y) { return 2 * (y * Width + x); }
    }
}
