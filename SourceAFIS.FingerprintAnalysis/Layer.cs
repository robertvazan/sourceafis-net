using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public enum Layer
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
