/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.minutiae;

import sourceafis.extraction.model.Ridge;
import sourceafis.extraction.model.SkeletonBuilder;
import sourceafis.extraction.model.SkeletonBuilderMinutia;
import sourceafis.general.Angle;
import sourceafis.general.DetailLogger;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;
import sourceafis.templates.Minutia;
import sourceafis.templates.MinutiaType;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class MinutiaCollector {
	@DpiAdjusted(min = 2)
	@Parameter(lower = 3, upper = 50)
	public int DirectionSegmentLength = 21;
	@DpiAdjusted(min = 0)
	@Parameter(lower = 0, upper = 20)
	public int DirectionSegmentSkip = 1;

	public DetailLogger.Hook Logger = DetailLogger.off;

	byte ComputeDirection(Ridge ridge) {
		int first = DirectionSegmentSkip;
		int last = DirectionSegmentSkip + DirectionSegmentLength - 1;

		if (last >= ridge.getPoints().size()) {
			int shift = last - ridge.getPoints().size() + 1;
			last -= shift;
			first -= shift;
		}
		if (first < 0)
			first = 0;

		return Angle.AtanB(ridge.getPoints().get(first),
				ridge.getPoints().get(last));
	}

	public void Collect(SkeletonBuilder skeleton, MinutiaType type,
			TemplateBuilder template) {
		for (SkeletonBuilderMinutia skeletonMinutia : skeleton.getMinutiae()) {
			if (skeletonMinutia.Valid
					&& skeletonMinutia.getRidges().size() == 1) {
				Minutia templateMinutia = new Minutia();
				templateMinutia.Type = type;
				templateMinutia.Position = skeletonMinutia.getPosition();
				templateMinutia.Direction = ComputeDirection(skeletonMinutia
						.getRidges().get(0));
				template.minutiae.add(templateMinutia);
			}
		}
		Logger.log(template);
	}
}
