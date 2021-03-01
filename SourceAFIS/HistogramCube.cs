// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	class HistogramCube
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int Bins;
		readonly int[] Counts;

		public HistogramCube(int width, int height, int bins) {
			Width = width;
			Height = height;
			Bins = bins;
			Counts = new int[width * height * bins];
		}
		public HistogramCube(IntPoint size, int bins) : this(size.X, size.Y, bins) { }

		public int this[int x, int y, int z]
		{
			get { return Counts[Offset(x, y, z)]; }
			set { Counts[Offset(x, y, z)] = value; }
		}
		public int this[IntPoint at, int z]
		{
			get { return this[at.X, at.Y, z]; }
			set { this[at.X, at.Y, z] = value; }
		}

		public int Constrain(int z) { return Math.Max(0, Math.Min(Bins - 1, z)); }
		public int Sum(int x, int y)
		{
			int sum = 0;
			for (int i = 0; i < Bins; ++i)
				sum += this[x, y, i];
			return sum;
		}
		public int Sum(IntPoint at) { return Sum(at.X, at.Y); }
		public void Add(int x, int y, int z, int value) { Counts[Offset(x, y, z)] += value; }
		public void Add(IntPoint at, int z, int value) { Add(at.X, at.Y, z, value); }
		public void Increment(int x, int y, int z) { Add(x, y, z, 1); }
		public void Increment(IntPoint at, int z) { Increment(at.X, at.Y, z); }
		int Offset(int x, int y, int z) { return (y * Width + x) * Bins + z; }
	}
}
