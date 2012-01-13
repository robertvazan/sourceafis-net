using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public enum BitmapLayer
    {
        None,
        OriginalImage,
        Equalized,
        SmoothedRidges,
        OrthogonalSmoothing,
        Binarized,
        BinarySmoothing,
        RemovedCrosses
    }
}
