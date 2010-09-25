using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    sealed class Options
    {
        public bool EnableImageDisplay = true;
        public LayerType DisplayLayer = LayerType.OriginalImage;
        public SkeletonType SkeletonType = SkeletonType.Ridges;
        public QuickCompare CompareWith = QuickCompare.None;
        public LayerType CompareWithLayer = LayerType.OriginalImage;
        public DiffType DiffType = DiffType.Proportional;
        public MaskType Mask = MaskType.None;
        public bool Contrast = false;
        public bool AbsoluteContrast = false;
        public bool RelativeContrast = false;
        public bool LowContrastMajority = false;
        public bool Orientation = false;
        public bool MinutiaCollector = false;
        public bool PairedMinutiae = false;
    }
}
