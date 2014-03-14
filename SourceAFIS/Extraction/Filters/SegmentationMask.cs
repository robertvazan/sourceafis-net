using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public static class SegmentationMask
    {
        static readonly VotingFilter LowContrastMajority = new VotingFilter(radius: 9, majority: 0.86f, borderDist: 7);
        static readonly VotingFilter BlockErrorFilter = new VotingFilter(majority: 0.7f, borderDist: 4);
        static readonly VotingFilter InnerMaskFilter = new VotingFilter(radius: 7, borderDist: 4);

        public static BinaryMap ComputeMask(BlockMap blocks, short[, ,] histogram)
        {
            byte[,] contrast = ClippedContrast.Compute(blocks, histogram);
            
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
