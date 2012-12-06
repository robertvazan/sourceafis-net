/**
 * @author Veaceslav Dubenco
 * @since 18.10.2012
 */
package sourceafis.extraction.minutiae;

import java.util.Iterator;

import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.meta.Parameter;
import sourceafis.templates.Minutia;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class MinutiaCloudRemover {
	@Parameter(upper = 300)
	public int NeighborhoodRadius = 20;
	@Parameter(upper = 30)
	public int MaxNeighbors = 4;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public void Filter(TemplateBuilder template) {
		int radiusSq = Calc.Sq(NeighborhoodRadius);
		Iterator<Minutia> iter = template.minutiae.iterator();
		while (iter.hasNext()) {
			Minutia minutia = iter.next();
			int count = 0;
			for (Minutia neighbor : template.minutiae) {
				if (Calc.DistanceSq(neighbor.Position, minutia.Position) <= radiusSq) {
					count++;
				}
			}
			if (count - 1 > MaxNeighbors) {
				iter.remove();
			}
		}
		Logger.log(template);
	}
}
