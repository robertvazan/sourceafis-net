/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;

/**
 * 
 */
public class PoreRemover implements ISkeletonFilter {

	@DpiAdjusted
	@Parameter(lower = 3, upper = 100)
	public int MaxArmLength = 41;

	@Nested
	public KnotRemover KnotRemover = new KnotRemover();

	public DetailLogger.Hook Logger = DetailLogger.off;

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			if (minutia.getRidges().size() == 3) {
				for (int exit = 0; exit < 3; ++exit) {
					Ridge exitRidge = minutia.getRidges().get(exit);
					Ridge arm1 = minutia.getRidges().get((exit + 1) % 3);
					Ridge arm2 = minutia.getRidges().get((exit + 2) % 3);
					if (arm1.getEnd() == arm2.getEnd()
							&& exitRidge.getEnd() != arm1.getEnd()
							&& arm1.getEnd() != minutia
							&& exitRidge.getEnd() != minutia) {
						SkeletonBuilderMinutia end = arm1.getEnd();
						if (end.getRidges().size() == 3
								&& arm1.getPoints().size() <= MaxArmLength
								&& arm2.getPoints().size() <= MaxArmLength) {
							arm1.Detach();
							arm2.Detach();
							Ridge merged = new Ridge();
							merged.setStart(minutia);
							merged.setEnd(end);
							for (Point point : Calc.ConstructLine(
									minutia.getPosition(), end.getPosition()))
								merged.Points.add(point);
						}
						break;
					}
				}
			}
		}
		KnotRemover.Filter(skeleton);
		Logger.log(skeleton);
	}
}
