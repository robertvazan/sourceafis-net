/**
 * @author Veaceslav Dubenco
 * @since 08.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.DetailLogger;
import sourceafis.meta.Parameter;

public final class AbsoluteContrast {
	DetailLogger.Hook logger = DetailLogger.off;

	@Parameter(upper = 255)
	public int Limit = 17;

	public BinaryMap DetectLowContrast(byte[][] contrast) {
		BinaryMap result = new BinaryMap(contrast[0].length, contrast.length);
		for (int y = 0; y < result.getHeight(); ++y)
			for (int x = 0; x < result.getWidth(); ++x)
				if ((contrast[y][x] & 0xFF) < Limit)
					result.SetBitOne(x, y);
		logger.log(result);
		return result;
	}
}
