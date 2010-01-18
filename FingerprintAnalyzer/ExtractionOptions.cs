using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    sealed class ExtractionOptions
    {
        public bool OriginalImage;
        public bool Equalized = false;
        public bool SmoothedRidges = false;
        public bool OrthogonalSmoothing = false;
        public bool Contrast = false;
        public bool AbsoluteContrast = false;
        public bool RelativeContrast = false;
        public bool LowContrastMajority = false;
        public bool SegmentationMask = false;
        public bool Orientation = false;
        public bool Binarized = false;
        public bool BinarySmoothing = false;
        public bool RemovedCrosses = false;
        public bool Thinned = false;
        public bool InnerMask = false;
        public SkeletonOptions Ridges = new SkeletonOptions();
        public SkeletonOptions Valleys = new SkeletonOptions();
        public bool MinutiaCollector = false;
    }
}
