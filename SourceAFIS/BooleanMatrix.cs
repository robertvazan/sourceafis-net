// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	class BooleanMatrix
	{
		public readonly int Width;
		public readonly int Height;
		readonly bool[] Cells;

		public IntPoint Size { get { return new IntPoint(Width, Height); } }

		public BooleanMatrix(int width, int height) {
			Width = width;
			Height = height;
			Cells = new bool[width * height];
		}
		public BooleanMatrix(IntPoint size) : this(size.X, size.Y) { }
		public BooleanMatrix(BooleanMatrix other)
			: this(other.size())
		{
			for (int i = 0; i < Cells.Length; ++i)
				Cells[i] = other.Cells[i];
		}

		public bool this[int x, int y]
		{
			get { return Cells[Offset(x, y)]; }
			set { Cells[Offset(x, y)] = value; }
		}
		public bool this[IntPoint at]
		{
			get { return this[at.X, at.Y]; }
			set { this[at.X, at.Y] = value; }
		}

		public bool Get(int x, int y, bool fallback)
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				return fallback;
			return Cells[Offset(x, y)];
		}
		public bool Get(IntPoint at, bool fallback) { return Get(at.X, at.Y, fallback); }
		public void Invert()
		{
			for (int i = 0; i < Cells.Length; ++i)
				Cells[i] = !Cells[i];
		}
		public void Merge(BooleanMatrix other) {
			if (other.Width != Width || other.Height != Height)
				throw new ArgumentException();
			for (int i = 0; i < Cells.Length; ++i)
				Cells[i] |= other.Cells[i];
		}
		int Offset(int x, int y) { return y * Width + x; }
	}
}
