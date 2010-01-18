using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    public sealed class ExtractionOptions
    {
        public bool OriginalImage;
        public bool Equalized;
        public bool SmoothedRidges;
        public bool OrthogonalSmoothing;
        public bool Contrast;
        public bool AbsoluteContrast;
        public bool RelativeContrast;
        public bool LowContrastMajority;
        public bool SegmentationMask;
        public bool Orientation;
        public bool Binarized;
        public bool BinarySmoothing;
        public bool RemovedCrosses;
        public bool Thinned;
        public bool InnerMask;
        public SkeletonOptions Ridges = new SkeletonOptions();
        public SkeletonOptions Valleys = new SkeletonOptions();
        public bool MinutiaCollector;
    }
}
