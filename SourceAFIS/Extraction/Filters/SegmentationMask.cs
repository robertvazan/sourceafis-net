using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class SegmentationMask
    {
        public ClippedContrast Contrast = new ClippedContrast();
        public AbsoluteContrast AbsoluteContrast = new AbsoluteContrast();
        public RelativeContrast RelativeContrast = new RelativeContrast();
        public VotingFilter LowContrastMajority = new VotingFilter(radius: 9, majority: 0.86f, borderDist: 7);
        public VotingFilter BlockErrorFilter = new VotingFilter(majority: 0.7f, borderDist: 4);
        public VotingFilter InnerMaskFilter = new VotingFilter(radius: 7, borderDist: 4);

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

            return mask;
        }
    }
}
