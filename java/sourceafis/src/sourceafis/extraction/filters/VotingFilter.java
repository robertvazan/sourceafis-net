/**
 * @author Veaceslav Dubenco
 * @since 18.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.general.Size;
import sourceafis.general.parallel.Parallel;
import sourceafis.general.parallel.ParallelForDelegate;
import sourceafis.meta.Parameter;

/**
 * 
 */
public final class VotingFilter {
	@Parameter(upper = 10)
	public int Radius = 1;
	@Parameter(lower = 0.51)
	public float Majority = 0.51f;
	@Parameter(lower = 0, upper = 20)
	public int BorderDistance = 0;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public BinaryMap Filter(final BinaryMap input) {
		final RectangleC rect = new RectangleC(new Point(BorderDistance,
				BorderDistance), new Size(
				input.getWidth() - 2 * BorderDistance, input.getHeight() - 2
						* BorderDistance));
		final BinaryMap output = new BinaryMap(input.getSize());
		ParallelForDelegate<Object> delegate = new ParallelForDelegate<Object>() {
			@Override
			public Object delegate(int y, Object inp) {
				for (int x = rect.getLeft(); x < rect.getRight(); ++x) {
					RectangleC neighborhood = new RectangleC(new Point(
							Math.max(x - Radius, 0), Math.max(y - Radius, 0)),
							new Point(Math.min(x + Radius + 1,
									output.getWidth()), Math.min(
									y + Radius + 1, output.getHeight())));

					int ones = 0;
					for (int ny = neighborhood.getBottom(); ny < neighborhood
							.getTop(); ++ny)
						for (int nx = neighborhood.getLeft(); nx < neighborhood
								.getRight(); ++nx)
							if (input.GetBit(nx, ny))
								++ones;

					double voteWeight = 1.0 / neighborhood.getTotalArea();
					if (ones * voteWeight >= Majority)
						output.SetBitOne(x, y);
				}
				return null;
			}

			@Override
			public Object combineResults(Object res1, Object res2) {
				return null;
			}
		};
		Parallel.For(rect.getRangeY().Begin, rect.getRangeY().End, delegate,
				null);
		Logger.log(output);
		return output;
	}
}
