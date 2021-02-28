// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
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

		public static implicit operator DoublePoint(IntPoint point) { return new DoublePoint(point.X, point.Y); }
		public static DoublePoint operator +(DoublePoint left, DoublePoint right) { return new DoublePoint(left.X + right.X, left.Y + right.Y); }
		public static DoublePoint operator -(DoublePoint left, DoublePoint right) { return new DoublePoint(left.X - right.X, left.Y - right.Y); }
		public static DoublePoint operator -(DoublePoint point) { return new DoublePoint(-point.X, -point.Y); }
		public static DoublePoint operator *(double factor, DoublePoint point) { return new DoublePoint(factor * point.X, factor * point.Y); }

		public override string ToString() { return string.Format("[{0},{1}]", X, Y); }
		public IntPoint Round() { return new IntPoint(Doubles.Round(X), Doubles.Round(Y)); }
	}
}
