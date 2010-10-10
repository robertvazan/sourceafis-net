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
        public SkeletonData(string context, Options.SkeletonType type)
        {
            RegisterProperties();
            LogStringDecoration = log => log + context;
            FpFilter = options => options.Path != "";
            Filter = options => options.Skeleton == type;
        }

        public LogProperty BinarizedProperty = new LogProperty("Extractor.Binarized")
        {
            Filter = options => false
        };
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        public LogProperty ThinnedProperty = new LogProperty("Extractor.Thinner")
        {
            Filter = options => options.UsesLayer(Options.Layer.Thinned)
        };
        public BinaryMap Thinned { get { return (BinaryMap)ThinnedProperty.Value; } }

        public LogProperty RidgeTracerProperty = new LogProperty("Extractor.RidgeTracer")
        {
            Filter = options => options.UsesLayer(Options.Layer.RidgeTracer)
        };
        public SkeletonBuilder RidgeTracer { get { return (SkeletonBuilder)RidgeTracerProperty.Value; } }

        public LogProperty DotRemoverProperty = new LogProperty("Extractor.DotRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.DotRemover)
        };
        public SkeletonBuilder DotRemover { get { return (SkeletonBuilder)DotRemoverProperty.Value; } }

        public LogProperty PoreRemoverProperty = new LogProperty("Extractor.PoreRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.PoreRemover)
        };
        public SkeletonBuilder PoreRemover { get { return (SkeletonBuilder)PoreRemoverProperty.Value; } }

        public LogProperty GapRemoverProperty = new LogProperty("Extractor.GapRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.GapRemover)
        };
        public SkeletonBuilder GapRemover { get { return (SkeletonBuilder)GapRemoverProperty.Value; } }

        public LogProperty TailRemoverProperty = new LogProperty("Extractor.TailRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.TailRemover)
        };
        public SkeletonBuilder TailRemover { get { return (SkeletonBuilder)TailRemoverProperty.Value; } }

        public LogProperty FragmentRemoverProperty = new LogProperty("Extractor.FragmentRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.FragmentRemover)
        };
        public SkeletonBuilder FragmentRemover { get { return (SkeletonBuilder)FragmentRemoverProperty.Value; } }

        public LogProperty MinutiaMaskProperty = new LogProperty("Extractor.MinutiaMask")
        {
            Filter = options => options.UsesLayer(Options.Layer.MinutiaMask)
        };
        public SkeletonBuilder MinutiaMask { get { return (SkeletonBuilder)MinutiaMaskProperty.Value; } }

        public LogProperty BranchMinutiaRemoverProperty = new LogProperty("Extractor.BranchMinutiaRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.BranchMinutiaRemover)
        };
        public SkeletonBuilder BranchMinutiaRemover { get { return (SkeletonBuilder)BranchMinutiaRemoverProperty.Value; } }
    }
}
