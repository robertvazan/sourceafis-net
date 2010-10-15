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

        public BinaryMap Binarized { get { return (BinaryMap)GetLog("Binarized", "Extractor.Binarized"); } }

        public BinaryMap Thinned { get { return (BinaryMap)GetLog("Thinned", "Extractor.Thinner"); } }

        public SkeletonBuilder RidgeTracer { get { return (SkeletonBuilder)GetLog("RidgeTracer", "Extractor.RidgeTracer"); } }

        public SkeletonBuilder DotRemover { get { return (SkeletonBuilder)GetLog("DotRemover", "Extractor.DotRemover"); } }

        public SkeletonBuilder PoreRemover { get { return (SkeletonBuilder)GetLog("PoreRemover", "Extractor.PoreRemover"); } }

        public SkeletonBuilder GapRemover { get { return (SkeletonBuilder)GetLog("GapRemover", "Extractor.GapRemover"); } }

        public SkeletonBuilder TailRemover { get { return (SkeletonBuilder)GetLog("TailRemover", "Extractor.TailRemover"); } }

        public SkeletonBuilder FragmentRemover { get { return (SkeletonBuilder)GetLog("FragmentRemover", "Extractor.FragmentRemover"); } }

        public SkeletonBuilder MinutiaMask { get { return (SkeletonBuilder)GetLog("MinutiaMask", "Extractor.MinutiaMask"); } }

        public SkeletonBuilder BranchMinutiaRemover { get { return (SkeletonBuilder)GetLog("BranchMinutiaRemover", "Extractor.BranchMinutiaRemover"); } }
    }
}
