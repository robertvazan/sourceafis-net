/**
 * @author Veaceslav Dubenco
 * @since 08.10.2012
 */
package sourceafis.general;

import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForDelegate;

/**
 * 
 */
public class BinaryMap implements Cloneable {
	private int WordWidth;

	public int getWordWidth() {
		return WordWidth;
	}

	private int Width;

	public int getWidth() {
		return Width;
	}

	private int Height;

	public int getHeight() {
		return Height;
	}

	public Size getSize() {
		return new Size(Width, Height);
	}

	public RectangleC getRect() {
		return new RectangleC(getSize());
	}

	public static final int WordShift = 5;
	public static final int WordMask = 31;
	public static final int WordSize = 32;
	public static final int WordBytes = WordSize / 8;

	private int[][] Map;

	public int[][] getMap() {
		return Map;
	}

	public BinaryMap(int width, int height) {
		Width = width;
		Height = height;
		WordWidth = (width + WordMask) >>> WordShift;
		Map = new int[height][WordWidth];
	}

	public BinaryMap(Size size) {
		this(size.Width, size.Height);
	}

	public BinaryMap(BinaryMap other) {
		Width = other.Width;
		Height = other.Height;
		WordWidth = other.WordWidth;
		Map = new int[other.Map.length][other.Map[0].length];
		for (int y = 0; y < Map.length; ++y)
			for (int x = 0; x < Map[0].length; ++x)
				Map[y][x] = other.Map[y][x];
	}

	@Override
	public Object clone() {
		return new BinaryMap(this);
	}

	public boolean IsWordNonZero(int xw, int y) {
		return Map[y][xw] != 0;
	}

	public boolean GetBit(int x, int y) {
		return (Map[y][x >>> WordShift] & (1 << (x & WordMask))) != 0;
	}

	public void SetBitOne(int x, int y) {
		Map[y][x >>> WordShift] |= 1 << (x & WordMask);
	}

	public void SetBitZero(int x, int y) {
		Map[y][x >>> WordShift] &= ~(1 << (x & WordMask));
	}

	public int GetWord(int xw, int y) {
		return Map[y][xw];
	}

	public void SetBit(int x, int y, boolean value) {
		if (value)
			SetBitOne(x, y);
		else
			SetBitZero(x, y);
	}

	public boolean GetBitSafe(int x, int y, boolean defaultValue) {
		if (getRect().Contains(new Point(x, y)))
			return GetBit(x, y);
		else
			return defaultValue;
	}

	public boolean GetBit(Point at) {
		return GetBit(at.X, at.Y);
	}

	public void SetBitOne(Point at) {
		SetBitOne(at.X, at.Y);
	}

	public void SetBitZero(Point at) {
		SetBitZero(at.X, at.Y);
	}

	public boolean GetBitSafe(Point at, boolean defaultValue) {
		return GetBitSafe(at.X, at.Y, defaultValue);
	}

	public void Clear() {
		for (int y = 0; y < Map.length; ++y)
			for (int x = 0; x < Map[0].length; ++x)
				Map[y][x] = 0;
	}

	public void Invert() {
		for (int y = 0; y < Map.length; ++y)
			for (int x = 0; x < Map[0].length; ++x)
				Map[y][x] = ~Map[y][x];
		if ((Width & WordMask) != 0)
			for (int y = 0; y < Map.length; ++y)
				Map[y][Map[0].length - 1] &= ~0 >>> (WordSize - (Width & WordMask));
	}

	public BinaryMap GetInverted() {
		BinaryMap result = new BinaryMap(getSize());
		for (int y = 0; y < Map.length; ++y)
			for (int x = 0; x < Map[0].length; ++x)
				result.Map[y][x] = ~Map[y][x];
		if ((Width & WordMask) != 0)
			for (int y = 0; y < Map.length; ++y)
				result.Map[y][Map[0].length - 1] &= ~0 >>> (WordSize - (Width & WordMask));
		return result;
	}

	public boolean IsEmpty() {
		for (int y = 0; y < Map.length; ++y)
			for (int x = 0; x < Map[0].length; ++x)
				if (Map[y][x] != 0)
					return false;
		return true;
	}

	static void ShiftLeft(int[] vector, int shift) {
		if (shift > 0) {
			for (int i = 0; i < vector.length - 1; ++i)
				vector[i] = (vector[i] >>> shift)
						| (vector[i + 1] << (WordSize - shift));
			vector[vector.length - 1] >>>= shift;
		}
	}

	static void ShiftRight(int[] vector, int shift) {
		if (shift > 0) {
			for (int i = vector.length - 1; i > 0; --i)
				vector[i] = (vector[i] << shift)
						| (vector[i - 1] >>> (WordSize - shift));
			vector[0] <<= shift;
		}
	}

	void LoadLine(int[] vector, Point at, int length) {
		int lastX = at.X + length - 1;
		int words = (lastX >>> WordShift) - (at.X >>> WordShift) + 1;
		for (int i = 0; i < words; ++i)
			vector[i] = Map[at.Y][(at.X >>> WordShift) + i];
		for (int i = words; i < vector.length; ++i)
			vector[i] = 0;
	}

	void SaveLine(int[] vector, Point at, int length) {
		int lastX = at.X + length - 1;
		int words = (lastX >>> WordShift) - (at.X >>> WordShift) + 1;
		for (int i = 1; i < words - 1; ++i)
			Map[at.Y][(at.X >>> WordShift) + i] = vector[i];

		int beginMask = ~0 << (at.X & WordMask);
		Map[at.Y][at.X >>> WordShift] = Map[at.Y][at.X >>> WordShift]
				& ~beginMask | vector[0] & beginMask;

		int endMask = ~0 >>> WordMask - (lastX & WordMask);
		Map[at.Y][lastX >>> WordShift] = Map[at.Y][lastX >>> WordShift]
				& ~endMask | vector[words - 1] & endMask;
	}

	interface CombineFunction {
		void combine(int[] target, int[] source);
	}

	class CombineLocals {
		public int[] Vector;
		public int[] SrcVector;
	}

	void Combine(final BinaryMap source, final RectangleC area, final Point at,
			final CombineFunction function) {
		final int shift = (area.X & WordMask) - (at.X & WordMask);
		int vectorSize = (area.Width >> WordShift) + 2;
		CombineLocals combineLocals = new CombineLocals();
		combineLocals.Vector = new int[vectorSize];
		combineLocals.SrcVector = new int[vectorSize];
		ParallelForDelegate<CombineLocals> delegate = new ParallelForDelegate<CombineLocals>() {
			@Override
			public CombineLocals delegate(int y, CombineLocals locals) {
				LoadLine(locals.Vector, new Point(at.X, at.Y + y), area.Width);
				source.LoadLine(locals.SrcVector,
						new Point(area.X, area.Y + y), area.Width);
				if (shift >= 0)
					ShiftLeft(locals.SrcVector, shift);
				else
					ShiftRight(locals.SrcVector, -shift);
				function.combine(locals.Vector, locals.SrcVector);
				SaveLine(locals.Vector, new Point(at.X, at.Y + y), area.Width);
				return locals;
			}

			@Override
			public CombineLocals combineResults(CombineLocals result1,
					CombineLocals result2) {
				return null;
			}
		};

		Parallel.For(0, area.Height, delegate, combineLocals);
	}

	public void Copy(BinaryMap source) {
		Copy(source, getRect(), new Point(0, 0));
	}

	public void Copy(final BinaryMap source, final RectangleC area,
			final Point at) {
		final int shift = (area.X & WordMask) - (at.X & WordMask);
		int[] vector = new int[(area.Width >> WordShift) + 2];
		ParallelForDelegate<int[]> delegate = new ParallelForDelegate<int[]>() {
			@Override
			public int[] delegate(int y, int[] vector) {
				source.LoadLine(vector, new Point(area.X, area.Y + y),
						area.Width);
				if (shift >= 0)
					ShiftLeft(vector, shift);
				else
					ShiftRight(vector, -shift);
				SaveLine(vector, new Point(at.X, at.Y + y), area.Width);
				return vector;
			}

			@Override
			public int[] combineResults(int[] result1, int[] result2) {
				return null;
			}
		};

		Parallel.For(0, area.Height, delegate, vector);
	}

	public void Or(BinaryMap source) {
		Or(source, getRect(), new Point(0, 0));
	}

	public void Or(BinaryMap source, RectangleC area, Point at) {
		Combine(source, area, at, new CombineFunction() {
			@Override
			public void combine(int[] target, int[] srcVector) {
				for (int i = 0; i < target.length; ++i)
					target[i] |= srcVector[i];
			}
		});
	}

	public void And(BinaryMap source) {
		And(source, getRect(), new Point(0, 0));
	}

	public void And(BinaryMap source, RectangleC area, Point at) {
		Combine(source, area, at, new CombineFunction() {
			@Override
			public void combine(int[] target, int[] srcVector) {
				for (int i = 0; i < target.length; ++i)
					target[i] &= srcVector[i];
			}
		});
	}

	public void Xor(BinaryMap source) {
		Xor(source, getRect(), new Point(0, 0));
	}

	public void Xor(BinaryMap source, RectangleC area, Point at) {
		Combine(source, area, at, new CombineFunction() {
			@Override
			public void combine(int[] target, int[] srcVector) {
				for (int i = 0; i < target.length; ++i)
					target[i] ^= srcVector[i];
			}
		});
	}

	public void OrNot(BinaryMap source) {
		OrNot(source, getRect(), new Point(0, 0));
	}

	public void OrNot(BinaryMap source, RectangleC area, Point at) {
		Combine(source, area, at, new CombineFunction() {
			@Override
			public void combine(int[] target, int[] srcVector) {
				for (int i = 0; i < target.length; ++i)
					target[i] |= ~srcVector[i];
			}
		});
	}

	public void AndNot(BinaryMap source) {
		AndNot(source, getRect(), new Point(0, 0));
	}

	public void AndNot(BinaryMap source, RectangleC area, Point at) {
		Combine(source, area, at, new CombineFunction() {
			@Override
			public void combine(int[] target, int[] srcVector) {
				for (int i = 0; i < target.length; ++i)
					target[i] &= ~srcVector[i];
			}
		});
	}

	public int GetNeighborhood(Point at) {
		return GetNeighborhood(at.X, at.Y);
	}

	public int GetNeighborhood(int x, int y) {
		if ((x & WordMask) >= 1 && (x & WordMask) <= 30) {
			int xWord = x >>> WordShift;
			int shift = x - 1 & WordMask;
			return ((Map[y + 1][xWord] >>> shift) & 7)
					| (((Map[y][xWord] >>> shift) & 1) << 3)
					| (((Map[y][xWord] >>> shift) & 4) << 2)
					| (((Map[y - 1][xWord] >>> shift) & 7) << 5);
		} else {
			int mask = 0;
			if (GetBit(x - 1, y + 1))
				mask |= 1;
			if (GetBit(x, y + 1))
				mask |= 2;
			if (GetBit(x + 1, y + 1))
				mask |= 4;
			if (GetBit(x - 1, y))
				mask |= 8;
			if (GetBit(x + 1, y))
				mask |= 16;
			if (GetBit(x - 1, y - 1))
				mask |= 32;
			if (GetBit(x, y - 1))
				mask |= 64;
			if (GetBit(x + 1, y - 1))
				mask |= 128;
			return mask;
		}
	}

	public void Fill(RectangleC rect) {
		if (rect.Width > 0) {
			int initialWord = rect.getLeft() >>> WordShift;
			int finalWord = (rect.getRight() - 1) >>> WordShift;
			int initialShift = rect.getLeft() & WordMask;
			int finalShift = 32 - (rect.getRight() & WordMask);
			for (int xw = initialWord; xw <= finalWord; ++xw) {
				int mask = ~0;
				if (xw == initialWord && initialShift != 0)
					mask = mask << initialShift;
				if (xw == finalWord && finalShift != WordSize)
					mask = (mask << finalShift) >>> finalShift;
				for (int y = rect.getBottom(); y < rect.getTop(); ++y)
					Map[y][xw] |= mask;
			}
		}
	}

	public BinaryMap FillBlocks(final BlockMap blocks) {
		final BinaryMap result = new BinaryMap(blocks.getPixelCount());

		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int blockY, Object input) {
				for (int blockX = 0; blockX < blocks.getBlockCount().Width; ++blockX)
					if (GetBit(blockX, blockY))
						result.Fill(blocks.getBlockAreas().get(blockY, blockX));
				return null;
			}

			@Override
			public Object combineResults(Object result1, Object result2) {
				return null;
			}
		};

		Parallel.For(0, blocks.getBlockCount().Height, delegate, null);

		return result;
	}

	public BinaryMap FillCornerAreas(final BlockMap blocks) {
		final BinaryMap result = new BinaryMap(blocks.getPixelCount());
		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int cornerY, Object input) {
				for (int cornerX = 0; cornerX < blocks.getCornerCount().Width; ++cornerX)
					if (GetBit(cornerX, cornerY))
						result.Fill(blocks.getCornerAreas().get(cornerY,
								cornerX));
				return null;
			}

			@Override
			public Object combineResults(Object result1, Object result2) {
				return null;
			}
		};
		Parallel.For(0, blocks.getCornerCount().Height, delegate, null);
		return result;
	}
	/*
	 * private boolean[][] Map; public int Width; public int Height;
	 * 
	 * public Size getSize() { return new Size(Width, Height); }
	 * 
	 * public RectangleC getRect() { return new RectangleC(getSize()); }
	 * 
	 * public BinaryMap(int width, int height) { Width = width; Height = height;
	 * Map = new boolean[Height][Width]; }
	 * 
	 * public BinaryMap(Size size) { this(size.Width, size.Height); }
	 * 
	 * public BinaryMap(BinaryMap other) { Width = other.Width; Height =
	 * other.Height; Map = new boolean[other.Map.length][other.Map[0].length];
	 * for (int y = 0; y < Map.length; ++y) for (int x = 0; x < Map[0].length;
	 * ++x) Map[y][x] = other.Map[y][x]; }
	 * 
	 * @Override public Object clone() { return new BinaryMap(this); }
	 * 
	 * public boolean GetBit(int x, int y) { return Map[y][x]; }
	 * 
	 * public void SetBitOne(int x, int y) { Map[y][x] = true; }
	 * 
	 * public void SetBitZero(int x, int y) { Map[y][x] = false; }
	 * 
	 * public void SetBit(int x, int y, boolean value) { if (value) SetBitOne(x,
	 * y); else SetBitZero(x, y); }
	 * 
	 * public boolean GetBitSafe(int x, int y, boolean defaultValue) { if
	 * (getRect().Contains(new Point(x, y))) return GetBit(x, y); else return
	 * defaultValue; }
	 * 
	 * public boolean GetBit(Point at) { return GetBit(at.X, at.Y); }
	 * 
	 * public void SetBitOne(Point at) { SetBitOne(at.X, at.Y); }
	 * 
	 * public void SetBitZero(Point at) { SetBitZero(at.X, at.Y); }
	 * 
	 * public boolean GetBitSafe(Point at, boolean defaultValue) { return
	 * GetBitSafe(at.X, at.Y, defaultValue); }
	 * 
	 * public void Clear() { for (int y = 0; y < Map.length; ++y) for (int x =
	 * 0; x < Map[0].length; ++x) Map[y][x] = false; }
	 * 
	 * public void Invert() { for (int y = 0; y < Map.length; ++y) for (int x =
	 * 0; x < Map[0].length; ++x) Map[y][x] = !Map[y][x]; }
	 * 
	 * public BinaryMap GetInverted() { BinaryMap result = new
	 * BinaryMap(getSize()); for (int y = 0; y < Map.length; ++y) for (int x =
	 * 0; x < Map[0].length; ++x) result.Map[y][x] = !Map[y][x]; return result;
	 * }
	 * 
	 * public boolean IsEmpty() { for (int y = 0; y < Map.length; ++y) for (int
	 * x = 0; x < Map[0].length; ++x) if (Map[y][x]) return false; return true;
	 * }
	 * 
	 * static void ShiftLeft(boolean[] vector, int shift) { if (shift > 0) { for
	 * (int i = 0; i < vector.length - shift; ++i) { vector[i] = vector[i +
	 * shift]; } for (int i = vector.length - shift; i < vector.length; ++i) {
	 * vector[i] = false; } } }
	 * 
	 * static void ShiftRight(boolean[] vector, int shift) { if (shift > 0) {
	 * for (int i = vector.length - 1; i >= shift; --i) { vector[i] = vector[i -
	 * shift]; } for (int i = shift - 1; i >= 0; --i) { vector[i] = false; } } }
	 * 
	 * void LoadLine(boolean[] vector, Point at, int length) { int lastX = at.X
	 * + length - 1; int words = lastX - at.X + 1; for (int i = 0; i < words;
	 * ++i) vector[i] = Map[at.Y][at.X + i]; for (int i = words; i <
	 * vector.length; ++i) vector[i] = false; }
	 * 
	 * void SaveLine(boolean[] vector, Point at, int length) { int lastX = at.X
	 * + length - 1; int words = lastX - at.X + 1; for (int i = 1; i < words -
	 * 1; ++i) { Map[at.Y][at.X + i] = vector[i]; } }
	 * 
	 * interface CombineFunction { void combine(int[] target, int[] source); }
	 * 
	 * class CombineLocals { public int[] Vector; public int[] SrcVector; }
	 * 
	 * void Combine(BinaryMap source, RectangleC area, Point at, CombineFunction
	 * function) { int shift = area.X - at.X; int vectorSize = area.Width + 2;
	 * Parallel.For(0, area.Height, () => new CombineLocals { Vector = new
	 * uint[vectorSize], SrcVector = new uint[vectorSize] }, delegate(int y,
	 * ParallelLoopState state, CombineLocals locals) { LoadLine(locals.Vector,
	 * new Point(at.X, at.Y + y), area.Width); source.LoadLine(locals.SrcVector,
	 * new Point(area.X, area.Y + y), area.Width); if (shift >= 0)
	 * ShiftLeft(locals.SrcVector, shift); else ShiftRight(locals.SrcVector,
	 * -shift); function(locals.Vector, locals.SrcVector);
	 * SaveLine(locals.Vector, new Point(at.X, at.Y + y), area.Width); return
	 * locals; }, locals => { }); }
	 * 
	 * public void Copy(BinaryMap source) { Copy(source, Rect, new Point()); }
	 * 
	 * public void Copy(BinaryMap source, RectangleC area, Point at) { int shift
	 * = (int)((uint)area.X & WordMask) - (int)((uint)at.X & WordMask);
	 * Parallel.For(0, area.Height, () => new uint[(area.Width >>> WordShift) +
	 * 2], delegate(int y, ParallelLoopState state, uint[] vector) {
	 * source.LoadLine(vector, new Point(area.X, area.Y + y), area.Width); if
	 * (shift >= 0) ShiftLeft(vector, shift); else ShiftRight(vector, -shift);
	 * SaveLine(vector, new Point(at.X, at.Y + y), area.Width); return vector;
	 * }, vector => { }); }
	 * 
	 * public void Or(BinaryMap source) { Or(source, Rect, new Point()); }
	 * 
	 * public void Or(BinaryMap source, RectangleC area, Point at) {
	 * Combine(source, area, at, delegate(uint[] target, uint[] srcVector) { for
	 * (int i = 0; i < target.Length; ++i) target[i] |= srcVector[i]; }); }
	 * 
	 * public void And(BinaryMap source) { And(source, Rect, new Point()); }
	 * 
	 * public void And(BinaryMap source, RectangleC area, Point at) {
	 * Combine(source, area, at, delegate(uint[] target, uint[] srcVector) { for
	 * (int i = 0; i < target.Length; ++i) target[i] &= srcVector[i]; }); }
	 * 
	 * public void Xor(BinaryMap source) { Xor(source, Rect, new Point()); }
	 * 
	 * public void Xor(BinaryMap source, RectangleC area, Point at) {
	 * Combine(source, area, at, delegate(uint[] target, uint[] srcVector) { for
	 * (int i = 0; i < target.Length; ++i) target[i] ^= srcVector[i]; }); }
	 * 
	 * public void OrNot(BinaryMap source) { OrNot(source, Rect, new Point()); }
	 * 
	 * public void OrNot(BinaryMap source, RectangleC area, Point at) {
	 * Combine(source, area, at, delegate(uint[] target, uint[] srcVector) { for
	 * (int i = 0; i < target.Length; ++i) target[i] |= ~srcVector[i]; }); }
	 * 
	 * public void AndNot(BinaryMap source) { AndNot(source, Rect, new Point());
	 * }
	 * 
	 * public void AndNot(BinaryMap source, RectangleC area, Point at) {
	 * Combine(source, area, at, delegate(uint[] target, uint[] srcVector) { for
	 * (int i = 0; i < target.Length; ++i) target[i] &= ~srcVector[i]; }); }
	 * 
	 * public uint GetNeighborhood(Point at) { return GetNeighborhood(at.X,
	 * at.Y); }
	 * 
	 * public uint GetNeighborhood(int x, int y) { if ((x & WordMask) >= 1 && (x
	 * & WordMask) <= 30) { int xWord = x >>> WordShift; int shift =
	 * (int)((uint)(x - 1) & WordMask); return ((Map[y + 1, xWord] >>> shift) &
	 * 7u) | (((Map[y, xWord] >>> shift) & 1u) << 3) | (((Map[y, xWord] >>>
	 * shift) & 4u) << 2) | (((Map[y - 1, xWord] >>> shift) & 7u) << 5); } else
	 * { uint mask = 0; if (GetBit(x - 1, y + 1)) mask |= 1; if (GetBit(x, y +
	 * 1)) mask |= 2; if (GetBit(x + 1, y + 1)) mask |= 4; if (GetBit(x - 1, y))
	 * mask |= 8; if (GetBit(x + 1, y)) mask |= 16; if (GetBit(x - 1, y - 1))
	 * mask |= 32; if (GetBit(x, y - 1)) mask |= 64; if (GetBit(x + 1, y - 1))
	 * mask |= 128; return mask; } }
	 * 
	 * public void Fill(RectangleC rect) { if (rect.Width > 0) { int initialWord
	 * = (int)((uint)rect.Left >>> WordShift); int finalWord = (rect.Right - 1)
	 * >>> WordShift; int initialShift = (int)((uint)rect.Left & WordMask); int
	 * finalShift = 32 - (int)((uint)rect.Right & WordMask); for (int xw =
	 * initialWord; xw <= finalWord; ++xw) { uint mask = ~0u; if (xw ==
	 * initialWord && initialShift != 0) mask = mask << initialShift; if (xw ==
	 * finalWord && finalShift != WordSize) mask = (mask << finalShift) >>>
	 * finalShift; for (int y = rect.Bottom; y < rect.Top; ++y) Map[y, xw] |=
	 * mask; } } }
	 * 
	 * public BinaryMap FillBlocks(BlockMap blocks) { BinaryMap result = new
	 * BinaryMap(blocks.PixelCount); Parallel.For(0, blocks.BlockCount.Height,
	 * delegate(int blockY) { for (int blockX = 0; blockX <
	 * blocks.BlockCount.Width; ++blockX) if (GetBit(blockX, blockY))
	 * result.Fill(blocks.BlockAreas[blockY, blockX]); }); return result; }
	 * 
	 * public BinaryMap FillCornerAreas(BlockMap blocks) { BinaryMap result =
	 * new BinaryMap(blocks.PixelCount); Parallel.For(0,
	 * blocks.CornerCount.Height, delegate(int cornerY) { for (int cornerX = 0;
	 * cornerX < blocks.CornerCount.Width; ++cornerX) if (GetBit(cornerX,
	 * cornerY)) result.Fill(blocks.CornerAreas[cornerY, cornerX]); }); return
	 * result; }
	 */
}
