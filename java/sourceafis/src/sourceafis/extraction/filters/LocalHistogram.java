/**
 * @author Veaceslav Dubenco
 * @since 17.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BlockMap;
import sourceafis.general.Calc;
import sourceafis.general.Neighborhood;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForEachDelegate;

/**
 * 
 */
public final class LocalHistogram {

	public short[][][] Analyze(final BlockMap blocks, final byte[][] image) {
		final short[][][] histogram = new short[blocks.getBlockCount().Height][blocks
				.getBlockCount().Width][256];
		/*ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point block) {
				RectangleC area = blocks.getBlockAreas().get(block);
				for (int y = area.getBottom(); y < area.getTop(); ++y)
					for (int x = area.getLeft(); x < area.getRight(); ++x)
						++histogram[block.Y][block.X][image[y][x] & 0xFF];
				return null;
			}

			@Override
			public Point combineResults(Point res1, Point res2) {
				return null;
			}
		};
		Parallel.ForEach(blocks.getAllBlocks(), delegate);
		*/
		for (Point block : blocks.getAllBlocks()) {
			RectangleC area = blocks.getBlockAreas().get(block);
			for (int y = area.getBottom(); y < area.getTop(); ++y)
				for (int x = area.getLeft(); x < area.getRight(); ++x)
					++histogram[block.Y][block.X][image[y][x] & 0xFF];
		}
		return histogram;
	}

	public short[][][] SmoothAroundCorners(final BlockMap blocks,
			final short[][][] input) {
		final Point[] blocksAround = new Point[] { new Point(0, 0),
				new Point(-1, 0), new Point(0, -1), new Point(-1, -1) };
		final short[][][] output = new short[blocks.getCornerCount().Height][blocks
				.getCornerCount().Width][256];

		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point corner) {
				for (Point relative : blocksAround) {
					Point block = Calc.Add(corner, relative);
					if (blocks.getAllBlocks().Contains(block)) {
						for (int i = 0; i < 256; ++i)
							output[corner.Y][corner.X][i] += input[block.Y][block.X][i];
					}
				}
				return null;
			}

			@Override
			public Point combineResults(Point res1, Point res2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllCorners(), delegate);
		return output;
	}

	public short[][][] Smooth(final BlockMap blocks, final short[][][] input) {
		final short[][][] output = new short[blocks.getCornerCount().Height][blocks
				.getCornerCount().Width][256];

		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point corner) {
				for (int i = 0; i < 256; ++i)
					output[corner.Y][corner.X][i] = input[corner.Y][corner.X][i];
				for (Point neigborRelative : Neighborhood.CornerNeighbors) {
					Point neighbor = Calc.Add(corner, neigborRelative);
					if (blocks.getAllCorners().contains(neighbor)) {
						for (int i = 0; i < 256; ++i)
							output[corner.Y][corner.X][i] += input[neighbor.Y][neighbor.X][i];
					}
				}
				return null;
			}

			@Override
			public Point combineResults(Point res1, Point res2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllCorners(), delegate);
		return output;
	}
}
