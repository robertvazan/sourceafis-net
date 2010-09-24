using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
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
        GapRemover,
        TailRemover,
        FragmentRemover,
        MinutiaMask,
        BranchMinutiaRemover
    }
}
