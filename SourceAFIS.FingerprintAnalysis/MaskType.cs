using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public enum MaskType
    {
        None,
        AbsoluteContrast,
        RelativeContrast,
        LowContrastMajority,
        Segmentation,
        Inner
    }
}
