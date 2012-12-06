/**
 * @author Veaceslav Dubenco
 * @since 17.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;

public final class InnerMask {
	@DpiAdjusted
	@Parameter(lower = 0, upper = 50)
	public int MinBorderDistance = 14;

	public DetailLogger.Hook Logger = DetailLogger.off;

	void ShrinkBy(BinaryMap temporary, BinaryMap inner, int amount) {
		temporary.Clear();
		temporary.Copy(inner, new RectangleC(amount, 0, inner.getWidth()
				- amount, inner.getHeight()), new Point(0, 0));
		temporary.And(inner, new RectangleC(0, 0, inner.getWidth() - amount,
				inner.getHeight()), new Point(amount, 0));
		temporary.And(inner,
				new RectangleC(0, amount, inner.getWidth(), inner.getHeight()
						- amount), new Point(0, 0));
		temporary.And(inner,
				new RectangleC(0, 0, inner.getWidth(), inner.getHeight()
						- amount), new Point(0, amount));
		inner.Copy(temporary);
	}

	public BinaryMap Compute(BinaryMap outer) {
		BinaryMap inner = new BinaryMap(outer.getSize());
		inner.Copy(outer,
				new RectangleC(1, 1, outer.getWidth() - 2,
						outer.getHeight() - 2), new Point(1, 1));
		BinaryMap temporary = new BinaryMap(outer.getSize());
		if (MinBorderDistance >= 1)
			ShrinkBy(temporary, inner, 1);
		int total = 1;
		for (int step = 1; total + step <= MinBorderDistance; step *= 2) {
			ShrinkBy(temporary, inner, step);
			total += step;
		}
		if (total < MinBorderDistance)
			ShrinkBy(temporary, inner, MinBorderDistance - total);
		Logger.log(inner);
		return inner;
	}
}
