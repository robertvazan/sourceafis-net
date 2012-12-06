/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.minutiae;

import java.util.Random;

import sourceafis.general.Calc;
import sourceafis.templates.Minutia;
import sourceafis.templates.TemplateBuilder;

/**
 * 
 */
public final class MinutiaShuffler {
	public void Shuffle(TemplateBuilder template) {
		int seed = 0;
		for (Minutia minutia : template.minutiae)
			seed += minutia.Direction + minutia.Position.X + minutia.Position.Y
					+ minutia.Type.getValue();
		template.minutiae = Calc.Shuffle(template.minutiae, new Random(seed));
	}

}
