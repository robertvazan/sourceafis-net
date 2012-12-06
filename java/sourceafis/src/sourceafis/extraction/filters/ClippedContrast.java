/**
 * @author Veaceslav Dubenco
 * @since 08.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BlockMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.meta.Parameter;

public final class ClippedContrast {
	@Parameter(upper = 0.4)
	public float ClipFraction = 0.08f;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public byte[][] Compute(final BlockMap blocks, final short[][][] histogram) {
		final byte[][] result = new byte[blocks.getBlockCount().Height][blocks
				.getBlockCount().Width];
		/*ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point block) {
				int area = 0;
				for (int i = 0; i < 256; ++i)
					area += histogram[block.Y][block.X][i];
				int clipLimit = Calc.toInt32(area * ClipFraction);

				int accumulator = 0;
				int lowerBound = 255;
				for (int i = 0; i < 256; ++i) {
					accumulator += histogram[block.Y][block.X][i];
					if (accumulator > clipLimit) {
						lowerBound = i;
						break;
					}
				}

				accumulator = 0;
				int upperBound = 0;
				for (int i = 255; i >= 0; --i) {
					accumulator += histogram[block.Y][block.X][i];
					if (accumulator > clipLimit) {
						upperBound = i;
						break;
					}
				}

				result[block.Y][block.X] = (byte) (upperBound - lowerBound);
				return null;
			}

			@Override
			public Point combineResults(Point result1, Point result2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllBlocks(), delegate);
		*/
		for (Point block : blocks.getAllBlocks()) {
			int area = 0;
			for (int i = 0; i < 256; ++i)
				area += histogram[block.Y][block.X][i];
			int clipLimit = Calc.toInt32(area * ClipFraction);

			int accumulator = 0;
			int lowerBound = 255;
			for (int i = 0; i < 256; ++i) {
				accumulator += histogram[block.Y][block.X][i];
				if (accumulator > clipLimit) {
					lowerBound = i;
					break;
				}
			}

			accumulator = 0;
			int upperBound = 0;
			for (int i = 255; i >= 0; --i) {
				accumulator += histogram[block.Y][block.X][i];
				if (accumulator > clipLimit) {
					upperBound = i;
					break;
				}
			}

			result[block.Y][block.X] = (byte) (upperBound - lowerBound);
		}

		return result;
	}
}
