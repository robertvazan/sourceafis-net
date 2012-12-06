/**
 * @author Veaceslav Dubenco
 * @since 16.10.2012
 */
package sourceafis.extraction.filters;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.Random;

import sourceafis.general.Angle;
import sourceafis.general.BinaryMap;
import sourceafis.general.BlockMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.PointF;
import sourceafis.general.Range;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForDelegate;
import sourceafis.general.parallel.ParallelForEachDelegate;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;

/**
 * 
 */
public final class HillOrientation {
	@DpiAdjusted
	@Parameter(lower = 0.5, upper = 4)
	public float MinHalfDistance = 2;
	@DpiAdjusted
	@Parameter(lower = 5, upper = 13)
	public float MaxHalfDistance = 6;
	@Parameter(upper = 100)
	public int NeighborListSplit = 50;
	@Parameter(upper = 100)
	public int NeighborsChecked = 20;
	@Parameter(upper = 3)
	public int SmoothingRadius = 1;

	public DetailLogger.Hook Logger = DetailLogger.off;

	class NeighborInfo {
		public Point Position;
		public PointF Orientation;

		@Override
		public boolean equals(Object o) {
			return (o instanceof NeighborInfo) && o != null
					&& this.Position.equals(((NeighborInfo) o).Position);
		}
	}

	List<List<NeighborInfo>> PrepareNeighbors() {
		Random random = new Random(0);
		List<List<NeighborInfo>> allSplits = new ArrayList<List<NeighborInfo>>();
		for (int i = 0; i < NeighborListSplit; ++i) {
			List<NeighborInfo> neighbors = new ArrayList<NeighborInfo>();
			for (int j = 0; j < NeighborsChecked; ++j) {
				NeighborInfo neighbor = new NeighborInfo();
				do {
					float angle = Angle.FromFraction((float) random
							.nextDouble() * 0.5f);
					float distance = Calc.InterpolateExponential(
							MinHalfDistance, MaxHalfDistance,
							(float) random.nextDouble());
					neighbor.Position = Calc.Round(Calc.Multiply(distance,
							Angle.ToVector(angle)));
				} while (neighbor.Position.equals(new Point(0, 0))
						|| neighbor.Position.Y < 0);
				neighbor.Orientation = Angle.ToVector(Angle.Add(
						Angle.ToOrientation(Angle.Atan(neighbor.Position)),
						Angle.PI));
				// if (!neighbors.Any(info => info.Position ==
				// neighbor.Position))
				if (!neighbors.contains(neighbor)) {
					neighbors.add(neighbor);
				}
			}
			Collections.sort(neighbors, new Comparator<NeighborInfo>() {
				@Override
				public int compare(NeighborInfo left, NeighborInfo right) {
					return Calc.CompareYX(left.Position, right.Position);
				}
			});
			// neighbors.Sort((left, right) => Calc.CompareYX(left.Position,
			// right.Position));
			allSplits.add(neighbors);
		}
		return allSplits;
	}

	Range GetMaskLineRange(BinaryMap mask, int y) {
		int first = -1;
		int last = -1;
		for (int x = 0; x < mask.getWidth(); ++x)
			if (mask.GetBit(x, y)) {
				last = x;
				if (first < 0)
					first = x;
			}
		if (first >= 0)
			return new Range(first, last + 1);
		else
			return new Range();
	}

	private void initPoinF2DArray(PointF[][] arr) {
		for (int i = 0; i < arr.length; i++) {
			for (int j = 0; j < arr[0].length; j++) {
				arr[i][j] = new PointF(0, 0);
			}
		}
	}

	PointF[][] AccumulateOrientations(final float[][] input,
			final BinaryMap mask, final BlockMap blocks) {
		final List<List<NeighborInfo>> neighbors = PrepareNeighbors();

		final PointF[][] orientation = new PointF[input.length][input[0].length];
		initPoinF2DArray(orientation);

		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int blockY, Object inp) {
				Range validMaskRange = GetMaskLineRange(mask, blockY);
				if (validMaskRange.getLength() > 0) {
					Range validXRange = new Range(blocks.getBlockAreas()
							.get(blockY, validMaskRange.Begin).getLeft(),
							blocks.getBlockAreas()
									.get(blockY, validMaskRange.End - 1)
									.getRight());
					for (int y = blocks.getBlockAreas().get(blockY, 0)
							.getBottom(); y < blocks.getBlockAreas()
							.get(blockY, 0).getTop(); ++y) {
						for (NeighborInfo neighbor : neighbors.get(y
								% neighbors.size())) {
							int radius = Math.max(
									Math.abs(neighbor.Position.X),
									Math.abs(neighbor.Position.Y));
							if (y - radius >= 0 && y + radius < input.length) {
								Range xRange = new Range(Math.max(radius,
										validXRange.Begin), Math.min(
										input[0].length - radius,
										validXRange.End));
								for (int x = xRange.Begin; x < xRange.End; ++x) {
									float before = input[y
											- neighbor.Position.Y][x
											- neighbor.Position.X];
									float at = input[y][x];
									float after = input[y + neighbor.Position.Y][x
											+ neighbor.Position.X];
									float strength = at
											- Math.max(before, after);
									if (strength > 0)
										orientation[y][x] = Calc.Add(
												orientation[y][x],
												Calc.Multiply(strength,
														neighbor.Orientation));
								}
							}
						}
					}
				}
				return null;
			}

			@Override
			public Object combineResults(Object result1, Object result2) {
				return null;
			}
		};
		Parallel.For(0, mask.getHeight(), delegate, null);
		Logger.log("Raw", orientation);
		return orientation;
	}

	PointF[][] SumBlocks(final PointF[][] orientation, final BlockMap blocks,
			final BinaryMap mask) {
		final PointF[][] sums = new PointF[blocks.getBlockCount().Height][blocks
				.getBlockCount().Width];
		initPoinF2DArray(sums);

		ParallelForEachDelegate<Point> delegate = new ParallelForEachDelegate<Point>() {
			@Override
			public Point delegate(Point block) {
				if (mask.GetBit(block)) {
					PointF sum = new PointF(0, 0);
					RectangleC area = blocks.getBlockAreas().get(block);
					for (int y = area.getBottom(); y < area.getTop(); ++y)
						for (int x = area.getLeft(); x < area.getRight(); ++x)
							sum = Calc.Add(sum, orientation[y][x]);
					sums[block.Y][block.X] = sum;
				}
				return null;
			}

			@Override
			public Point combineResults(Point result1, Point result2) {
				return null;
			}
		};

		Parallel.ForEach(blocks.getAllBlocks(), delegate);
		return sums;
	}

	PointF[][] Smooth(final PointF[][] orientation, final BinaryMap mask) {
		final PointF[][] smoothed = new PointF[mask.getHeight()][mask
				.getWidth()];
		initPoinF2DArray(smoothed);

		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int y, Object inp) {
				for (int x = 0; x < mask.getWidth(); ++x)
					if (mask.GetBit(x, y)) {
						RectangleC neighbors = new RectangleC(new Point(
								Math.max(0, x - SmoothingRadius), Math.max(0, y
										- SmoothingRadius)), new Point(
								Math.min(mask.getWidth(), x + SmoothingRadius
										+ 1), Math.min(mask.getHeight(), y
										+ SmoothingRadius + 1)));
						PointF sum = new PointF(0, 0);
						for (int ny = neighbors.getBottom(); ny < neighbors
								.getTop(); ++ny)
							for (int nx = neighbors.getLeft(); nx < neighbors
									.getRight(); ++nx)
								if (mask.GetBit(nx, ny))
									sum = Calc.Add(sum, orientation[ny][nx]);
						smoothed[y][x] = sum;
					}
				return null;
			}

			@Override
			public Object combineResults(Object result1, Object result2) {
				return null;
			}
		};

		Parallel.For(0, mask.getHeight(), delegate, null);
		return smoothed;
	}

	byte[][] ToAngles(final PointF[][] vectors, final BinaryMap mask) {
		final byte[][] angles = new byte[mask.getHeight()][mask.getWidth()];

		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int y, Object inp) {
				for (int x = 0; x < mask.getWidth(); ++x)
					if (mask.GetBit(x, y))
						angles[y][x] = Angle.ToByte(Angle.Atan(vectors[y][x]));
				return null;
			}

			@Override
			public Object combineResults(Object result1, Object result2) {
				return null;
			}
		};
		Parallel.For(0, mask.getHeight(), delegate, null);
		return angles;
	}

	public byte[][] Detect(float[][] image, BinaryMap mask, BlockMap blocks) {
		PointF[][] accumulated = AccumulateOrientations(image, mask, blocks);
		PointF[][] byBlock = SumBlocks(accumulated, blocks, mask);
		PointF[][] smooth = Smooth(byBlock, mask);
		byte[][] angles = ToAngles(smooth, mask);
		Logger.log(angles);
		return angles;
	}
}
