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
        public SkeletonData()
        {
            RegisterProperties();
        }

        LogProperty BinarizedProperty = new LogProperty();
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        LogProperty ThinnedProperty = new LogProperty();
        public BinaryMap Thinned { get { return (BinaryMap)ThinnedProperty.Value; } }

        LogProperty RidgeTracerProperty = new LogProperty();
        public SkeletonBuilder RidgeTracer { get { return (SkeletonBuilder)RidgeTracerProperty.Value; } }

        LogProperty DotRemoverProperty = new LogProperty();
        public SkeletonBuilder DotRemover { get { return (SkeletonBuilder)DotRemoverProperty.Value; } }

        LogProperty PoreRemoverProperty = new LogProperty();
        public SkeletonBuilder PoreRemover { get { return (SkeletonBuilder)PoreRemoverProperty.Value; } }

        LogProperty GapRemoverProperty = new LogProperty();
        public SkeletonBuilder GapRemover { get { return (SkeletonBuilder)GapRemoverProperty.Value; } }

        LogProperty TailRemoverProperty = new LogProperty();
        public SkeletonBuilder TailRemover { get { return (SkeletonBuilder)TailRemoverProperty.Value; } }

        LogProperty FragmentRemoverProperty = new LogProperty();
        public SkeletonBuilder FragmentRemover { get { return (SkeletonBuilder)FragmentRemoverProperty.Value; } }

        LogProperty MinutiaMaskProperty = new LogProperty();
        public SkeletonBuilder MinutiaMask { get { return (SkeletonBuilder)MinutiaMaskProperty.Value; } }

        LogProperty BranchMinutiaRemoverProperty = new LogProperty();
        public SkeletonBuilder BranchMinutiaRemover { get { return (SkeletonBuilder)BranchMinutiaRemoverProperty.Value; } }
    }
}
