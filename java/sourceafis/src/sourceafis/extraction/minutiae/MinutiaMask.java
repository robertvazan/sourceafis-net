/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.minutiae;

import java.util.Iterator;

import sourceafis.general.Angle;
import sourceafis.general.BinaryMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.PointF;
import sourceafis.general.Size;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;
import sourceafis.templates.Minutia;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class MinutiaMask {
	@DpiAdjusted(min = 0)
	@Parameter(lower = 0, upper = 50)
	public float DirectedExtension = 10.06f;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public void Filter(TemplateBuilder template, BinaryMap mask) {
		Iterator<Minutia> iter = template.minutiae.iterator();
		while (iter.hasNext()) {
			Minutia minutia = iter.next();
			Point arrow = Calc.Round(PointF.multiply(-DirectedExtension,
					Angle.ToVector(minutia.Direction)));
			if (!mask.GetBitSafe(Point.add(minutia.Position, new Size(arrow)),
					false)) {
				iter.remove();
			}
		}
		Logger.log(template);
	}
}
