/**
 * @author Veaceslav Dubenco
 * @since 17.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.Angle;
import sourceafis.general.BinaryMap;
import sourceafis.general.BlockMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForEachDelegate;
import sourceafis.meta.Nested;

/**
 * 
 */
public final class OrientedSmoother {
	public byte AngleOffset;
	@Nested
	public LinesByOrientation Lines = new LinesByOrientation();

	public DetailLogger.Hook Logger = DetailLogger.off;

	public float[][] Smooth(final float[][] input, final byte[][] orientation,
			final BinaryMap mask, final BlockMap blocks) {
		final Point[][] lines = Lines.Construct();
		final float[][] output = new float[input.length][input[0].length];

		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point block) {
				if (mask.GetBit(block)) {
					Point[] line = lines[Angle.Quantize(Angle.Add(
							orientation[block.Y][block.X], AngleOffset),
							lines.length)];
					for (Point linePoint : line) {
						RectangleC target = blocks.getBlockAreas().get(block);
						RectangleC source = target.GetShifted(linePoint);
						source.Clip(new RectangleC(blocks.getPixelCount()));
						target = source.GetShifted(Calc.Negate(linePoint));
						for (int y = target.getBottom(); y < target.getTop(); ++y)
							for (int x = target.getLeft(); x < target
									.getRight(); ++x)
								output[y][x] += input[y + linePoint.Y][x
										+ linePoint.X];
					}
					RectangleC blockArea = blocks.getBlockAreas().get(block);
					for (int y = blockArea.getBottom(); y < blockArea.getTop(); ++y)
						for (int x = blockArea.getLeft(); x < blockArea
								.getRight(); ++x)
							output[y][x] *= 1f / line.length;
				}
				return null;
			}

			@Override
			public Point combineResults(Point res1, Point res2) {
				return null;
			}
		};
		Parallel.ForEach(blocks.getAllBlocks(), delegate);
		Logger.log(output);

		return output;
	}
}
