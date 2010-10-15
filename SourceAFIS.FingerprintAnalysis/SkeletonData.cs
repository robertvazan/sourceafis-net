using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.FingerprintAnalysis
{
    public class SkeletonData : LogData
    {
        public SkeletonData(string context)
        {
            LogStringDecoration = log => log + context;
        }

        public BinaryMap Binarized { get { return (BinaryMap)GetLog("Binarized", "Binarized"); } }

        public BinaryMap Thinned { get { return (BinaryMap)GetLog("Thinned", "Thinner"); } }

        public SkeletonBuilder RidgeTracer { get { return (SkeletonBuilder)GetLog("RidgeTracer", "RidgeTracer"); } }

        public SkeletonBuilder DotRemover { get { return (SkeletonBuilder)GetLog("DotRemover", "DotRemover"); } }

        public SkeletonBuilder PoreRemover { get { return (SkeletonBuilder)GetLog("PoreRemover", "PoreRemover"); } }

        public SkeletonBuilder GapRemover { get { return (SkeletonBuilder)GetLog("GapRemover", "GapRemover"); } }

        public SkeletonBuilder TailRemover { get { return (SkeletonBuilder)GetLog("TailRemover", "TailRemover"); } }

        public SkeletonBuilder FragmentRemover { get { return (SkeletonBuilder)GetLog("FragmentRemover", "FragmentRemover"); } }

        public SkeletonBuilder MinutiaMask { get { return (SkeletonBuilder)GetLog("MinutiaMask", "MinutiaMask"); } }

        public SkeletonBuilder BranchMinutiaRemover { get { return (SkeletonBuilder)GetLog("BranchMinutiaRemover", "BranchMinutiaRemover"); } }
    }
}
