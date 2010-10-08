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

        LogProperty InputImageProperty = new LogProperty();
        public byte[,] InputImage { get { return (byte[,])InputImageProperty.Value; } }

        LogProperty BlocksProperty = new LogProperty();
        public BlockMap Blocks { get { return (BlockMap)BlocksProperty.Value; } }

        LogProperty BlockContrastProperty = new LogProperty();
        public byte[,] BlockContrast { get { return (byte[,])BlockContrastProperty.Value; } }

        LogProperty AbsoluteContrastProperty = new LogProperty();
        public BinaryMap AbsoluteContrast { get { return (BinaryMap)AbsoluteContrastProperty.Value; } }

        LogProperty RelativeContrastProperty = new LogProperty();
        public BinaryMap RelativeContrast { get { return (BinaryMap)RelativeContrastProperty.Value; } }

        LogProperty LowContrastMajorityProperty = new LogProperty();
        public BinaryMap LowContrastMajority { get { return (BinaryMap)LowContrastMajorityProperty.Value; } }

        LogProperty SegmentationMaskProperty = new LogProperty();
        public BinaryMap SegmentationMask { get { return (BinaryMap)SegmentationMaskProperty.Value; } }

        LogProperty EqualizedProperty = new LogProperty();
        public float[,] Equalized { get { return (float[,])EqualizedProperty.Value; } }

        LogProperty OrientationProperty = new LogProperty();
        public byte[,] Orientation { get { return (byte[,])OrientationProperty.Value; } }

        LogProperty SmoothedRidgesProperty = new LogProperty();
        public float[,] SmoothedRidges { get { return (float[,])SmoothedRidgesProperty.Value; } }

        LogProperty OrthogonalSmoothingProperty = new LogProperty();
        public float[,] OrthogonalSmoothing { get { return (float[,])OrthogonalSmoothingProperty.Value; } }

        LogProperty BinarizedProperty = new LogProperty();
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        LogProperty BinarySmoothingProperty = new LogProperty();
        public BinaryMap BinarySmoothing { get { return (BinaryMap)BinarySmoothingProperty.Value; } }

        LogProperty RemovedCrossesProperty = new LogProperty();
        public BinaryMap RemovedCrosses { get { return (BinaryMap)RemovedCrossesProperty.Value; } }

        LogProperty InnerMaskProperty = new LogProperty();
        public BinaryMap InnerMask { get { return (BinaryMap)InnerMaskProperty.Value; } }

        LogProperty MinutiaCollectorProperty = new LogProperty();
        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)MinutiaCollectorProperty.Value; } }

        LogProperty TemplateProperty = new LogProperty();
        public Template Template { get { return (Template)TemplateProperty.Value; } }
    }
}
