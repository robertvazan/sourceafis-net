/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.extraction.Extractor;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.meta.Nested;

/**
 * 
 */
public class KnotRemover implements ISkeletonFilter {

	@Nested
	public DotRemover DotRemover = new DotRemover();

	public DetailLogger.Hook Logger = DetailLogger.off;

	private void swap(Ridge r1, Ridge r2) {
		Ridge tmp = new Ridge();
		tmp.setContent(r1);
		r1.setContent(r2);
		r2.setContent(tmp);
	}

	public static volatile int cnt = 0;

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			if (minutia.getRidges().size() == 2
					&& minutia.getRidges().get(0).getReversed() != minutia
							.getRidges().get(1)) {
				Ridge extended = minutia.getRidges().get(0).getReversed();
				Ridge removed = minutia.getRidges().get(1);
				if (extended.getPoints().size() < removed.getPoints().size()) {
					//swap(extended, removed);
					extended = extended.getReversed();
					removed = removed.getReversed();
				}

				extended.getPoints().remove(extended.getPoints().size() - 1);
				for (Point point : removed.getPoints())
					extended.getPoints().add(point);

				extended.setEnd(removed.getEnd());
				if (Extractor.checkNullRidgeEnd("BeforeDetach", skeleton)) {
					System.out.println("before");
				}
				cnt++;
				removed.Detach();
				if (Extractor.checkNullRidgeEnd("AfterDetach", skeleton)) {
					System.out.println("after: " + (cnt - 1));
				}
			}
		}
		Extractor.checkNullRidgeEnd("KnotRemoverIn", skeleton);
		DotRemover.Filter(skeleton);
		Logger.log(skeleton);
	}
}
