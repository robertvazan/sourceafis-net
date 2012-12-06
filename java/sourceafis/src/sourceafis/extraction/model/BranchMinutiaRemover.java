/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.general.DetailLogger;

/**
 * 
 */
public class BranchMinutiaRemover implements ISkeletonFilter {

	public DetailLogger.Hook Logger = DetailLogger.off;

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			if (minutia.getRidges().size() > 2)
				minutia.Valid = false;
		}
		Logger.log(skeleton);
	}

}
