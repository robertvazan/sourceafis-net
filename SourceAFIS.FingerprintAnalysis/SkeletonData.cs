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
            RegisterProperties();
            LogStringDecoration = log => log + context;
        }

        public LogProperty BinarizedProperty = new LogProperty("Extractor.Binarized");
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        public LogProperty ThinnedProperty = new LogProperty("Extractor.Thinner");
        public BinaryMap Thinned { get { return (BinaryMap)ThinnedProperty.Value; } }

        public LogProperty RidgeTracerProperty = new LogProperty("Extractor.RidgeTracer");
        public SkeletonBuilder RidgeTracer { get { return (SkeletonBuilder)RidgeTracerProperty.Value; } }

        public LogProperty DotRemoverProperty = new LogProperty("Extractor.DotRemover");
        public SkeletonBuilder DotRemover { get { return (SkeletonBuilder)DotRemoverProperty.Value; } }

        public LogProperty PoreRemoverProperty = new LogProperty("Extractor.PoreRemover");
        public SkeletonBuilder PoreRemover { get { return (SkeletonBuilder)PoreRemoverProperty.Value; } }

        public LogProperty GapRemoverProperty = new LogProperty("Extractor.GapRemover");
        public SkeletonBuilder GapRemover { get { return (SkeletonBuilder)GapRemoverProperty.Value; } }

        public LogProperty TailRemoverProperty = new LogProperty("Extractor.TailRemover");
        public SkeletonBuilder TailRemover { get { return (SkeletonBuilder)TailRemoverProperty.Value; } }

        public LogProperty FragmentRemoverProperty = new LogProperty("Extractor.FragmentRemover");
        public SkeletonBuilder FragmentRemover { get { return (SkeletonBuilder)FragmentRemoverProperty.Value; } }

        public LogProperty MinutiaMaskProperty = new LogProperty("Extractor.MinutiaMask");
        public SkeletonBuilder MinutiaMask { get { return (SkeletonBuilder)MinutiaMaskProperty.Value; } }

        public LogProperty BranchMinutiaRemoverProperty = new LogProperty("Extractor.BranchMinutiaRemover");
        public SkeletonBuilder BranchMinutiaRemover { get { return (SkeletonBuilder)BranchMinutiaRemoverProperty.Value; } }
    }
}
