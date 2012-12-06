/**
 * @author Veaceslav Dubenco
 * @since 09.10.2012
 */
package sourceafis.general;

/**
 * 
 */
public final class BlockMap {
	public final class PointGrid {
		private int[] AllX;
		private int[] AllY;

		public PointGrid(Size size) {
			AllX = new int[size.Width];
			AllY = new int[size.Height];
		}

		public int[] getAllX() {
			return AllX;
		}

		public int[] getAllY() {
			return AllY;
		}

		public Point get(int y, int x) {
			return new Point(AllX[x], AllY[y]);
		}

		public Point get(Point at) {
			return new Point(AllX[at.X], AllY[at.Y]);
		}
	}

	public final class RectangleGrid {
		private PointGrid Corners;

		public RectangleGrid(PointGrid corners) {
			Corners = corners;
		}

		public PointGrid getCorners() {
			return Corners;
		}

		public RectangleC get(int y, int x) {
			return new RectangleC(Corners.get(y, x), Corners.get(y + 1, x + 1));
		}

		public RectangleC get(Point at) {
			return new RectangleC(Corners.get(at), Corners.get(at.Y + 1,
					at.X + 1));
		}
	}

	private Size PixelCount;
	private Size BlockCount;
	private Size CornerCount;
	private RectangleC AllBlocks;
	private RectangleC AllCorners;
	private PointGrid Corners;
	private RectangleGrid BlockAreas;
	private PointGrid BlockCenters;
	private RectangleGrid CornerAreas;

	public BlockMap(Size pixelSize, int maxBlockSize) {
		PixelCount = pixelSize;
		BlockCount = new Size(Calc.DivRoundUp(PixelCount.Width, maxBlockSize),
				Calc.DivRoundUp(PixelCount.Height, maxBlockSize));
		CornerCount = BlockToCornerCount(BlockCount);

		AllBlocks = new RectangleC(BlockCount);
		AllCorners = new RectangleC(CornerCount);

		Corners = InitCorners();
		BlockAreas = new RectangleGrid(Corners);
		BlockCenters = InitBlockCenters();
		CornerAreas = InitCornerAreas();
	}

	public Size getPixelCount() {
		return PixelCount;
	}

	public Size getBlockCount() {
		return BlockCount;
	}

	public Size getCornerCount() {
		return CornerCount;
	}

	public RectangleC getAllBlocks() {
		return AllBlocks;
	}

	public RectangleC getAllCorners() {
		return AllCorners;
	}

	public PointGrid getCorners() {
		return Corners;
	}

	public RectangleGrid getBlockAreas() {
		return BlockAreas;
	}

	public PointGrid getBlockCenters() {
		return BlockCenters;
	}

	public RectangleGrid getCornerAreas() {
		return CornerAreas;
	}

	static Size BlockToCornerCount(Size BlockCount) {
		return new Size(BlockCount.Width + 1, BlockCount.Height + 1);
	}

	static Size CornerToBlockCount(Size CornerCount) {
		return new Size(CornerCount.Width - 1, CornerCount.Height - 1);
	}

	PointGrid InitCorners() {
		PointGrid grid = new PointGrid(CornerCount);
		for (int y = 0; y < CornerCount.Height; ++y)
			grid.AllY[y] = y * PixelCount.Height / BlockCount.Height;
		for (int x = 0; x < CornerCount.Width; ++x)
			grid.AllX[x] = x * PixelCount.Width / BlockCount.Width;
		return grid;
	}

	PointGrid InitBlockCenters() {
		PointGrid grid = new PointGrid(BlockCount);
		for (int y = 0; y < BlockCount.Height; ++y)
			grid.AllY[y] = BlockAreas.get(y, 0).getCenter().Y;
		for (int x = 0; x < BlockCount.Width; ++x)
			grid.AllX[x] = BlockAreas.get(0, x).getCenter().X;
		return grid;
	}

	RectangleGrid InitCornerAreas() {
		PointGrid grid = new PointGrid(new Size(CornerCount.Width + 1,
				CornerCount.Height + 1));

		grid.AllY[0] = 0;
		for (int y = 0; y < BlockCount.Height; ++y)
			grid.AllY[y + 1] = BlockCenters.get(y, 0).Y;
		grid.AllY[BlockCount.Height] = PixelCount.Height;

		grid.AllX[0] = 0;
		for (int x = 0; x < BlockCount.Width; ++x)
			grid.AllX[x + 1] = BlockCenters.get(0, x).X;
		grid.AllX[BlockCount.Width] = PixelCount.Width;

		return new RectangleGrid(grid);
	}
}
