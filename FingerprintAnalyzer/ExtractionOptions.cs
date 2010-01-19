using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    sealed class ExtractionOptions
    {
        public LayerType DisplayLayer = LayerType.OriginalImage;
        public SkeletonType SkeletonType = SkeletonType.Ridges;
        public LayerType CompareWith = LayerType.OriginalImage;
        public bool Contrast = false;
        public bool AbsoluteContrast = false;
        public bool RelativeContrast = false;
        public bool LowContrastMajority = false;
        public bool SegmentationMask = false;
        public bool Orientation = false;
        public bool InnerMask = false;
        public bool MinutiaCollector = false;
    }
}
