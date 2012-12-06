/**
 * @author Veaceslav Dubenco
 * @since 18.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.BlockMap;
import sourceafis.general.DetailLogger;
import sourceafis.meta.Nested;

/**
 * 
 */
public final class SegmentationMask {
	@Nested
	public ClippedContrast Contrast = new ClippedContrast();
	@Nested
	public AbsoluteContrast AbsoluteContrast = new AbsoluteContrast();
	@Nested
	public RelativeContrast RelativeContrast = new RelativeContrast();
	@Nested
	public VotingFilter LowContrastMajority = new VotingFilter();
	@Nested
	public VotingFilter BlockErrorFilter = new VotingFilter();
	@Nested
	public VotingFilter InnerMaskFilter = new VotingFilter();

	public DetailLogger.Hook Logger = DetailLogger.off;

	public SegmentationMask() {
		LowContrastMajority.BorderDistance = 7;
		LowContrastMajority.Radius = 9;
		LowContrastMajority.Majority = 0.86f;
		BlockErrorFilter.BorderDistance = 4;
		BlockErrorFilter.Majority = 0.7f;
		InnerMaskFilter.Radius = 7;
		InnerMaskFilter.BorderDistance = 4;
	}

	public BinaryMap ComputeMask(BlockMap blocks, short[][][] histogram) {
		byte[][] contrast = Contrast.Compute(blocks, histogram);

		BinaryMap mask = new BinaryMap(
				AbsoluteContrast.DetectLowContrast(contrast));
		mask.Or(RelativeContrast.DetectLowContrast(contrast, blocks));
		mask.Or(LowContrastMajority.Filter(mask));

		mask.Or(BlockErrorFilter.Filter(mask));
		mask.Invert();
		mask.Or(BlockErrorFilter.Filter(mask));
		mask.Or(BlockErrorFilter.Filter(mask));
		mask.Or(InnerMaskFilter.Filter(mask));

		Logger.log(mask);
		return mask;
	}
}
