/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.minutiae;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.meta.Parameter;
import sourceafis.templates.Minutia;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class UniqueMinutiaSorter {
	@Parameter(lower = 25, upper = 1000)
	public int MaxMinutiae = 100;
	@Parameter(upper = 20)
	public int NeighborhoodSize = 5;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public void Filter(TemplateBuilder template) {
		if (template.minutiae.size() > MaxMinutiae) {
			template.minutiae = getSortedMinutiaList(template);
		}
		Logger.log(template);
	}

	private List<Minutia> getSortedMinutiaList(TemplateBuilder template) {
		List<MinutiaRadius> mrList = new ArrayList<MinutiaRadius>();
		for (Minutia minutia : template.minutiae) {
			mrList.add(new MinutiaRadius(minutia, getNeighborDistance(minutia,
					template)));
		}
		Collections.sort(mrList);
		List<Minutia> ret = new ArrayList<Minutia>();
		for (int i = 0; i < MaxMinutiae; i++) {
			ret.add(mrList.get(i).minutia);
		}
		return ret;
	}

	private Integer getNeighborDistance(Minutia minutia,
			TemplateBuilder template) {
		List<Integer> ret = new ArrayList<Integer>();
		for (Minutia neighbor : template.minutiae) {
			ret.add(Calc.DistanceSq(minutia.Position, neighbor.Position));
		}
		Collections.sort(ret);
		return ret.get(NeighborhoodSize);
	}

	class MinutiaRadius implements Comparable<MinutiaRadius> {
		Minutia minutia;
		int radius;

		public MinutiaRadius(Minutia minutia, int radius) {
			this.minutia = minutia;
			this.radius = radius;
		}

		@Override
		public int compareTo(MinutiaRadius obj) {
			return obj.radius - this.radius;
		}

		@Override
		public boolean equals(Object obj) {
			if (obj instanceof MinutiaRadius) {
				return (this.radius == ((MinutiaRadius) obj).radius);
			} else {
				return false;
			}
		}
	}
}
