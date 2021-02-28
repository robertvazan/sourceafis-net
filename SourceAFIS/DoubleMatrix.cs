// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	class DoubleMatrix
	{
		public readonly int Width;
		public readonly int Height;
		readonly double[] Cells;

		public IntPoint Size { get { return new IntPoint(Width, Height); } }

		public DoubleMatrix(int width, int height) {
			Width = width;
			Height = height;
			Cells = new double[width * height];
		}
		public DoubleMatrix(IntPoint size) : this(size.X, size.Y) { }

		public double this[int x, int y]
		{
			get { return Cells[Offset(x, y)]; }
			set { Cells[Offset(x, y)] = value; }
		}
		public double this[IntPoint at]
		{
			get { return this[at.X, at.Y]; }
			set { this[at.X, at.Y] = value; }
		}

		public void Add(int x, int y, double value) { Cells[Offset(x, y)] += value; }
		public void Add(IntPoint at, double value) { Add(at.X, at.Y, value); }
		public void Multiply(int x, int y, double value) { Cells[Offset(x, y)] *= value; }
		public void Multiply(IntPoint at, double value) { Multiply(at.X, at.Y, value); }
		int Offset(int x, int y) { return y * Width + x; }
	}
}
