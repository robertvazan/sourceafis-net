using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    enum LayerType
    {
        OriginalImage,
        Equalized,
        SmoothedRidges,
        OrthogonalSmoothing,
        Binarized,
        BinarySmoothing,
        RemovedCrosses,
        Thinned,
        RidgeTracer,
        DotRemover,
        PoreRemover,
        TailRemover,
        FragmentRemover,
        MinutiaMask
    }
}
