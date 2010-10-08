using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ExtractionData : LogData
    {
        public ExtractionData()
        {
            RegisterProperties();
        }

        public SkeletonData Ridges = new SkeletonData();
        public SkeletonData Valleys = new SkeletonData();

        public byte[,] InputImage { get; set; }

        public LogProperty BlocksProperty = new LogProperty();
        public BlockMap Blocks { get { return (BlockMap)BlocksProperty.Value; } }

        public LogProperty BlockContrastProperty = new LogProperty();
        public byte[,] BlockContrast { get { return (byte[,])BlockContrastProperty.Value; } }

        public LogProperty AbsoluteContrastProperty = new LogProperty();
        public BinaryMap AbsoluteContrast { get { return (BinaryMap)AbsoluteContrastProperty.Value; } }

        public LogProperty RelativeContrastProperty = new LogProperty();
        public BinaryMap RelativeContrast { get { return (BinaryMap)RelativeContrastProperty.Value; } }

        public LogProperty LowContrastMajorityProperty = new LogProperty();
        public BinaryMap LowContrastMajority { get { return (BinaryMap)LowContrastMajorityProperty.Value; } }

        public LogProperty SegmentationMaskProperty = new LogProperty();
        public BinaryMap SegmentationMask { get { return (BinaryMap)SegmentationMaskProperty.Value; } }

        public LogProperty EqualizedProperty = new LogProperty();
        public float[,] Equalized { get { return (float[,])EqualizedProperty.Value; } }

        public LogProperty OrientationProperty = new LogProperty();
        public byte[,] Orientation { get { return (byte[,])OrientationProperty.Value; } }

        public LogProperty SmoothedRidgesProperty = new LogProperty();
        public float[,] SmoothedRidges { get { return (float[,])SmoothedRidgesProperty.Value; } }

        public LogProperty OrthogonalSmoothingProperty = new LogProperty();
        public float[,] OrthogonalSmoothing { get { return (float[,])OrthogonalSmoothingProperty.Value; } }

        public LogProperty BinarizedProperty = new LogProperty();
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        public LogProperty BinarySmoothingProperty = new LogProperty();
        public BinaryMap BinarySmoothing { get { return (BinaryMap)BinarySmoothingProperty.Value; } }

        public LogProperty RemovedCrossesProperty = new LogProperty();
        public BinaryMap RemovedCrosses { get { return (BinaryMap)RemovedCrossesProperty.Value; } }

        public LogProperty InnerMaskProperty = new LogProperty();
        public BinaryMap InnerMask { get { return (BinaryMap)InnerMaskProperty.Value; } }

        public LogProperty MinutiaCollectorProperty = new LogProperty();
        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)MinutiaCollectorProperty.Value; } }

        public ComputedProperty TemplateProperty = new ComputedProperty("MinutiaCollector");
        public Template Template { get { return new SerializedFormat().Export(MinutiaCollector); } }
    }
}
