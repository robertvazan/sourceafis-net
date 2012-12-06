/**
 * @author Veaceslav Dubenco
 * @since 18.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Neighborhood;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForDelegate;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;

/**
 * 
 */
public final class Thinner {
	@DpiAdjusted
	@Parameter(lower = 5, upper = 50)
	public int MaxIterations = 26;

	public DetailLogger.Hook Logger = DetailLogger.off;

	// TODO: getters
	private static boolean[] IsRemovable;
	private static boolean[] IsEnding;

	static {
		IsRemovable = new boolean[256];
		IsEnding = new boolean[256];
		for (int mask = 0; mask < 256; ++mask) {
			boolean TL = (mask & 1) != 0;
			boolean TC = (mask & 2) != 0;
			boolean TR = (mask & 4) != 0;
			boolean CL = (mask & 8) != 0;
			boolean CR = (mask & 16) != 0;
			boolean BL = (mask & 32) != 0;
			boolean BC = (mask & 64) != 0;
			boolean BR = (mask & 128) != 0;

			int count = Calc.CountBits(mask);

			boolean diagonal = !TC && !CL && TL || !CL && !BC && BL || !BC
					&& !CR && BR || !CR && !TC && TR;
			boolean horizontal = !TC && !BC && (TR || CR || BR)
					&& (TL || CL || BL);
			boolean vertical = !CL && !CR && (TL || TC || TR)
					&& (BL || BC || BR);
			boolean end = (count == 1);

			IsRemovable[mask] = !diagonal && !horizontal && !vertical && !end;
			IsEnding[mask] = end;
		}
	}

	static boolean IsFalseEnding(BinaryMap binary, Point ending) {
		for (Point relativeNeighbor : Neighborhood.CornerNeighbors) {
			Point neighbor = Calc.Add(ending, relativeNeighbor);
			if (binary.GetBit(neighbor))
				return Calc.CountBits(binary.GetNeighborhood(neighbor)) > 2;
		}
		return false;
	}

	public BinaryMap Thin(final BinaryMap input) {
		final BinaryMap intermediate = new BinaryMap(input.getSize());
		intermediate.Copy(input, new RectangleC(1, 1, input.getWidth() - 2,
				input.getHeight() - 2), new Point(1, 1));

		final BinaryMap border = new BinaryMap(input.getSize());
		final BinaryMap skeleton = new BinaryMap(input.getSize());
		boolean removedAnything = true;
		for (int i = 0; i < MaxIterations && removedAnything; ++i) {
			removedAnything = false;
			for (int j = 0; j < 4; ++j) {
				border.Copy(intermediate);
				switch (j) {
				case 0:
					border.AndNot(
							intermediate,
							new RectangleC(1, 0, border.getWidth() - 1, border
									.getHeight()), new Point(0, 0));
					break;
				case 1:
					border.AndNot(
							intermediate,
							new RectangleC(0, 0, border.getWidth() - 1, border
									.getHeight()), new Point(1, 0));
					break;
				case 2:
					border.AndNot(
							intermediate,
							new RectangleC(0, 1, border.getWidth(), border
									.getHeight() - 1), new Point(0, 0));
					break;
				case 3:
					border.AndNot(
							intermediate,
							new RectangleC(0, 0, border.getWidth(), border
									.getHeight() - 1), new Point(0, 1));
					break;
				}
				border.AndNot(skeleton);

				ParallelForDelegate<Integer> delegate = new ParallelForDelegate<Integer>() {
					@Override
					public Integer delegate(int y, Integer odd) {
						int removedCount = 0;
						if (y % 2 == odd)
							for (int xw = 0; xw < input.getWordWidth(); ++xw)
								if (border.IsWordNonZero(xw, y))
									for (int x = xw << BinaryMap.WordShift; x < (xw << BinaryMap.WordShift)
											+ BinaryMap.WordSize; ++x)
										if (x > 0 && x < input.getWidth() - 1
												&& border.GetBit(x, y)) {
											int neighbors = intermediate
													.GetNeighborhood(x, y);
											if (IsRemovable[neighbors]
													|| IsEnding[neighbors]
													&& IsFalseEnding(
															intermediate,
															new Point(x, y))) {
												removedCount++;
												intermediate.SetBitZero(x, y);
											} else
												skeleton.SetBitOne(x, y);
										}
						return removedCount;
					}

					@Override
					public Integer combineResults(Integer res1, Integer res2) {
						if (res1 == null) {
							res1 = 0;
						}
						if (res2 == null) {
							res2 = 0;
						}
						return res1 + res2;
					}
				};

				for (int odd = 0; odd < 2; ++odd)
					if (Parallel.For(1, input.getHeight() - 1, delegate, odd) > 0) {
						removedAnything = true;
					}
			}
		}

		Logger.log(skeleton);
		return skeleton;
	}
}
