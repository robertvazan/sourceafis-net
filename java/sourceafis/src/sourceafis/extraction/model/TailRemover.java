/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.general.DetailLogger;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;

/**
 * 
 */
public class TailRemover implements ISkeletonFilter {
	@DpiAdjusted
	@Parameter(lower = 3, upper = 100)
	public int MinTailLength = 21;

	@Nested
	public DotRemover DotRemover = new DotRemover();
	@Nested
	public KnotRemover KnotRemover = new KnotRemover();

	public DetailLogger.Hook Logger = DetailLogger.off;

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			if (minutia.getRidges().size() == 1
					&& minutia.getRidges().get(0).getEnd().getRidges().size() >= 3)
				if (minutia.getRidges().get(0).Points.size() < MinTailLength)
					minutia.getRidges().get(0).Detach();
		}
		DotRemover.Filter(skeleton);
		KnotRemover.Filter(skeleton);
		Logger.log(skeleton);
	}
}
