using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction
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

        public SegmentationMask()
        {
            LowContrastMajority.Radius = 4;
            LowContrastMajority.Majority = 0.6f;
            InnerMaskFilter.Radius = 5;
            InnerMaskFilter.BorderDistance = 5;
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

            Logger.Log(this, mask);
            return mask;
        }
    }
}
