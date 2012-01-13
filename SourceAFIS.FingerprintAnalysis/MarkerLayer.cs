using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public enum MarkerLayer
    {
        None,
        Thinned,
        RidgeTracer,
        DotRemover,
        PoreRemover,
        GapRemover,
        TailRemover,
        FragmentRemover,
        BranchMinutiaRemover,
        MinutiaCollector,
        MinutiaMask,
        MinutiaCloudRemover,
        UniqueMinutiaSorter
    }
}
