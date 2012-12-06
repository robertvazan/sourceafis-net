/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.minutiae;

import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;
import sourceafis.templates.Minutia;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class StandardDpiScaling {
	@DpiAdjusted
	@Parameter(lower = 500, upper = 500)
	public int DpiScaling = 500;

	public DetailLogger.Hook Logger = DetailLogger.off;

	public void Scale(TemplateBuilder template) {
		float dpiFactor = 500 / (float) DpiScaling;
		for (Minutia minutia : template.minutiae)
			minutia.Position = Calc.Multiply(dpiFactor, minutia.Position);
		Logger.log(template);
	}
}
