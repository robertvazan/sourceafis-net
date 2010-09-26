using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public enum LayerType
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
