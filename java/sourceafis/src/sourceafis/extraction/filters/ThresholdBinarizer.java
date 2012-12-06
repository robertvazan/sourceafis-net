/**
 * @author Veaceslav Dubenco
 * @since 18.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.BlockMap;
import sourceafis.general.DetailLogger;
import sourceafis.general.RectangleC;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForDelegate;

/**
 * 
 */
public final class ThresholdBinarizer {
	public DetailLogger.Hook Logger = DetailLogger.off;

	public BinaryMap Binarize(final float[][] input, final float[][] baseline,
			final BinaryMap mask, final BlockMap blocks) {
		final BinaryMap binarized = new BinaryMap(input[0].length, input.length);

		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int blockY, Object obj) {
				for (int blockX = 0; blockX < blocks.getAllBlocks().Width; ++blockX) {
					if (mask.GetBit(blockX, blockY)) {
						RectangleC rect = blocks.getBlockAreas().get(blockY,
								blockX);
						for (int y = rect.getBottom(); y < rect.getTop(); ++y)
							for (int x = rect.getLeft(); x < rect.getRight(); ++x)
								if (input[y][x] - baseline[y][x] > 0)
									binarized.SetBitOne(x, y);
					}
				}
				return null;
			}

			@Override
			public Object combineResults(Object res1, Object res2) {
				return null;
			}
		};

		Parallel.For(0, blocks.getAllBlocks().Height, delegate, null);
		Logger.log(binarized);
		return binarized;
	}
}
