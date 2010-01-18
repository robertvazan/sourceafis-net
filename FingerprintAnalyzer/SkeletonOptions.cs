using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    sealed class SkeletonOptions
    {
        public bool Binarized = false;
        public bool Thinned = false;
        public bool RidgeTracer = false;
        public bool DotRemover = false;
        public bool PoreRemover = false;
        public bool TailRemover = false;
        public bool FragmentRemover = false;
        public bool MinutiaMask = false;
        public bool ShowEndings = false;
    }
}
