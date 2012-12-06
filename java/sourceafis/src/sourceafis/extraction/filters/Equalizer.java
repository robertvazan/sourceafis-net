/**
 * @author Veaceslav Dubenco
 * @since 15.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.BlockMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.PointF;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForEachDelegate;
import sourceafis.meta.Parameter;

/**
 * 
 */
public final class Equalizer {
	@Parameter(lower = 1, upper = 10)
	public float MaxScaling = 3.99f;
	@Parameter(lower = 0.1)
	public float MinScaling = 0.25f;

	public DetailLogger.Hook Logger = DetailLogger.off;

	private static final float RangeMin = -1;
	private static final float RangeMax = 1;
	private static final float RangeSize = RangeMax - RangeMin;

	private static float[] ToFloatTable;

	public float[] getToFloatTable() {
		return ToFloatTable;
	}

	static {
		ToFloatTable = new float[256];
		for (int i = 0; i < 256; ++i)
			ToFloatTable[i] = i / 255f;
	}

	float[][][] ComputeEqualization(final BlockMap blocks,
			final short[][][] histogram, final BinaryMap blockMask) {
		float widthMax = RangeSize / 256f * MaxScaling;
		float widthMin = RangeSize / 256f * MinScaling;

		final float[] limitedMin = new float[256];
		final float[] limitedMax = new float[256];
		for (int i = 0; i < 256; ++i) {
			limitedMin[i] = Math.max(i * widthMin + RangeMin, RangeMax
					- (255 - i) * widthMax);
			limitedMax[i] = Math.min(i * widthMax + RangeMin, RangeMax
					- (255 - i) * widthMin);
		}

		final float[][][] equalization = new float[blocks.getCornerCount().Height][blocks
				.getCornerCount().Width][256];

		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point corner) {
				if (blockMask.GetBitSafe(corner.X, corner.Y, false)
						|| blockMask.GetBitSafe(corner.X - 1, corner.Y, false)
						|| blockMask.GetBitSafe(corner.X, corner.Y - 1, false)
						|| blockMask.GetBitSafe(corner.X - 1, corner.Y - 1,
								false)) {
					int area = 0;
					for (int i = 0; i < 256; ++i)
						area += histogram[corner.Y][corner.X][i];
					float widthWeigth = RangeSize / area;

					float top = RangeMin;
					for (int i = 0; i < 256; ++i) {
						float width = histogram[corner.Y][corner.X][i]
								* widthWeigth;
						float equalized = top + ToFloatTable[i] * width;
						top += width;

						float limited = equalized;
						if (limited < limitedMin[i])
							limited = limitedMin[i];
						if (limited > limitedMax[i])
							limited = limitedMax[i];
						equalization[corner.Y][corner.X][i] = limited;
					}
				}
				return null;
			}

			@Override
			public Point combineResults(Point result1, Point result2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllCorners(), delegate);
		return equalization;
	}

	float[][] PerformEqualization(final BlockMap blocks, final byte[][] image,
			final float[][][] equalization, final BinaryMap blockMask) {
		final float[][] result = new float[blocks.getPixelCount().Height][blocks
				.getPixelCount().Width];
		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point block) {
				if (blockMask.GetBit(block)) {
					RectangleC area = blocks.getBlockAreas().get(block);
					for (int y = area.getBottom(); y < area.getTop(); ++y)
						for (int x = area.getLeft(); x < area.getRight(); ++x) {
							int pixel = image[y][x] & 0xFF;

							float bottomLeft = equalization[block.Y][block.X][pixel];
							float bottomRight = equalization[block.Y][block.X + 1][pixel];
							float topLeft = equalization[block.Y + 1][block.X][pixel];
							float topRight = equalization[block.Y + 1][block.X + 1][pixel];

							PointF fraction = area.GetFraction(new Point(x, y));
							result[y][x] = Calc.Interpolate(topLeft, topRight,
									bottomLeft, bottomRight, fraction);
						}
				}
				return null;
			}

			@Override
			public Point combineResults(Point result1, Point result2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllBlocks(), delegate);
		Logger.log(result);
		return result;
	}

	public float[][] Equalize(BlockMap blocks, byte[][] image,
			short[][][] histogram, BinaryMap blockMask) {
		float[][][] equalization = ComputeEqualization(blocks, histogram,
				blockMask);
		return PerformEqualization(blocks, image, equalization, blockMask);
	}
}
