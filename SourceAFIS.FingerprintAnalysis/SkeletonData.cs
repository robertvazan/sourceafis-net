using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.FingerprintAnalysis
{
    public class SkeletonData
    {
        public BinaryMap Binarized;
        public BinaryMap Thinned;
        public SkeletonBuilder RidgeTracer;
        public SkeletonBuilder DotRemover;
        public SkeletonBuilder PoreRemover;
        public SkeletonBuilder GapRemover;
        public SkeletonBuilder TailRemover;
        public SkeletonBuilder FragmentRemover;
        public SkeletonBuilder MinutiaMask;
        public SkeletonBuilder BranchMinutiaRemover;
    }
}
