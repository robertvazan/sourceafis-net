// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    readonly struct DoublePoint
    {
        public static readonly DoublePoint Zero = new DoublePoint();

        public readonly double X;
        public readonly double Y;

        public DoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator DoublePoint(IntPoint point) => new DoublePoint(point.X, point.Y);
        public static DoublePoint operator +(DoublePoint left, DoublePoint right) => new DoublePoint(left.X + right.X, left.Y + right.Y);
        public static DoublePoint operator -(DoublePoint left, DoublePoint right) => new DoublePoint(left.X - right.X, left.Y - right.Y);
        public static DoublePoint operator -(DoublePoint point) => new DoublePoint(-point.X, -point.Y);
        public static DoublePoint operator *(double factor, DoublePoint point) => new DoublePoint(factor * point.X, factor * point.Y);

        public override string ToString() => $"[{X},{Y}]";
        public IntPoint Round() => new IntPoint(Doubles.RoundToInt(X), Doubles.RoundToInt(Y));
    }
}
