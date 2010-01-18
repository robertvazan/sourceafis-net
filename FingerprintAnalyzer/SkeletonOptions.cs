using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    public sealed class SkeletonOptions
    {
        public bool Binarized;
        public bool Thinned;
        public bool RidgeTracer;
        public bool DotRemover;
        public bool PoreRemover;
        public bool TailRemover;
        public bool FragmentRemover;
        public bool MinutiaMask;
        public bool ShowEndings;
    }
}
