/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import java.util.Iterator;

import sourceafis.general.DetailLogger;

/**
 * 
 */
public class DotRemover implements ISkeletonFilter {

	public DetailLogger.Hook Logger = DetailLogger.off;

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		Iterator<SkeletonBuilderMinutia> iter = skeleton.getMinutiae()
				.iterator();
		while (iter.hasNext()) {
			SkeletonBuilderMinutia minutia = iter.next();
			if (minutia.getRidges().size() == 0) {
				iter.remove();
			}
		}
		Logger.log(skeleton);
	}
}
