using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class SegmentationMask
    {
        [Nested]
        public ClippedContrast Contrast = new ClippedContrast();
        [Nested]
        public AbsoluteContrast AbsoluteContrast = new AbsoluteContrast();
        [Nested]
        public RelativeContrast RelativeContrast = new RelativeContrast();
        [Nested]
        public VotingFilter LowContrastMajority = new VotingFilter();
        [Nested]
        public VotingFilter BlockErrorFilter = new VotingFilter();
        [Nested]
        public VotingFilter InnerMaskFilter = new VotingFilter();

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public SegmentationMask()
        {
            LowContrastMajority.BorderDistance = 0;
            LowContrastMajority.Radius = 6;
            LowContrastMajority.Majority = 0.86f;
            BlockErrorFilter.BorderDistance = 5;
            BlockErrorFilter.Majority = 0.7f;
            InnerMaskFilter.Radius = 7;
            InnerMaskFilter.BorderDistance = 4;
        }

        public BinaryMap ComputeMask(BlockMap blocks, short[, ,] histogram)
        {
            byte[,] contrast = Contrast.Compute(blocks, histogram);
            
            BinaryMap mask = new BinaryMap(AbsoluteContrast.DetectLowContrast(contrast));
            mask.Or(RelativeContrast.DetectLowContrast(contrast, blocks));
            mask.Or(LowContrastMajority.Filter(mask));
            
            mask.Or(BlockErrorFilter.Filter(mask));
            mask.Invert();
            mask.Or(BlockErrorFilter.Filter(mask));
            mask.Or(BlockErrorFilter.Filter(mask));
            mask.Or(InnerMaskFilter.Filter(mask));

            Logger.Log(mask);
            return mask;
        }
    }
}
