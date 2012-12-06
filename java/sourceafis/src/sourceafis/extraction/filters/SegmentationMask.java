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
		DetailLogger.log2D(contrast, "contrast_j.csv");

		BinaryMap mask = new BinaryMap(
				AbsoluteContrast.DetectLowContrast(contrast));
		DetailLogger.log(mask, "1Contr_j.csv");
		BinaryMap relative = RelativeContrast.DetectLowContrast(contrast,
				blocks);
		DetailLogger.log(relative, "2Contr_j.csv");
		mask.Or(relative);
		DetailLogger.log(mask, "3Contr_j.csv");
		BinaryMap maj = LowContrastMajority.Filter(mask);
		DetailLogger.log(maj, "4Contr_j.csv");
		mask.Or(LowContrastMajority.Filter(mask));
		DetailLogger.log(mask, "5Contr_j.csv");
		BinaryMap blerr = BlockErrorFilter.Filter(mask);
		DetailLogger.log(mask, "6Contr_j.csv");
		mask.Or(blerr);
		DetailLogger.log(mask, "7Contr_j.csv");
		mask.Invert();
		DetailLogger.log(mask, "8Contr_j.csv");
		mask.Or(BlockErrorFilter.Filter(mask));
		mask.Or(BlockErrorFilter.Filter(mask));
		BinaryMap inner = InnerMaskFilter.Filter(mask);
		DetailLogger.log(inner, "9Contr_j.csv");
		mask.Or(inner);
		DetailLogger.log(mask, "mask_j.csv");

		//Logger.Log(mask);
		return mask;
	}
}
