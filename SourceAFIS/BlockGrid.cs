// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	class BlockGrid
	{
		public readonly IntPoint Blocks;
		public readonly IntPoint Corners;
		public readonly int[] X;
		public readonly int[] Y;

		public BlockGrid(IntPoint size)
		{
			Blocks = size;
			Corners = new IntPoint(size.x + 1, size.y + 1);
			X = new int[size.x + 1];
			Y = new int[size.y + 1];
		}
		public BlockGrid(int width, int height) : this(new IntPoint(width, height)) { }

		public IntPoint Corner(int atX, int atY) { return new IntPoint(X[atX], Y[atY]); }
		public IntPoint Corner(IntPoint at) { return Corner(at.x, at.y); }
		public IntRect Block(int atX, int atY) { return IntRect.Between(Corner(atX, atY), Corner(atX + 1, atY + 1)); }
		public IntRect Block(IntPoint at) { return Block(at.x, at.y); }
	}
}
