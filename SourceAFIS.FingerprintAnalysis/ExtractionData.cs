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
            FpFilter = options => options.Path != "";
        }

        public SkeletonData Ridges = new SkeletonData("[Ridges]", Options.SkeletonType.Ridges);
        public SkeletonData Valleys = new SkeletonData("[Valleys]", Options.SkeletonType.Valleys);

        public byte[,] InputImage { get; set; }

        public LogProperty BlocksProperty = new LogProperty("Extractor.BlockMap");
        public BlockMap Blocks { get { return (BlockMap)BlocksProperty.Value; } }

        public LogProperty BlockContrastProperty = new LogProperty("Extractor.Mask.Contrast");
        public byte[,] BlockContrast { get { return (byte[,])BlockContrastProperty.Value; } }

        public LogProperty AbsoluteContrastProperty = new LogProperty("Extractor.Mask.AbsoluteContrast");
        public BinaryMap AbsoluteContrast { get { return (BinaryMap)AbsoluteContrastProperty.Value; } }

        public LogProperty RelativeContrastProperty = new LogProperty("Extractor.Mask.RelativeContrast");
        public BinaryMap RelativeContrast { get { return (BinaryMap)RelativeContrastProperty.Value; } }

        public LogProperty LowContrastMajorityProperty = new LogProperty("Extractor.Mask.LowContrastMajority");
        public BinaryMap LowContrastMajority { get { return (BinaryMap)LowContrastMajorityProperty.Value; } }

        public LogProperty SegmentationMaskProperty = new LogProperty("Extractor.Mask");
        public BinaryMap SegmentationMask { get { return (BinaryMap)SegmentationMaskProperty.Value; } }

        public LogProperty EqualizedProperty = new LogProperty("Extractor.Equalizer")
        {
            Filter = options => options.UsesLayer(Options.Layer.Equalized)
        };
        public float[,] Equalized { get { return (float[,])EqualizedProperty.Value; } }

        public LogProperty OrientationProperty = new LogProperty("Extractor.Orientation");
        public byte[,] Orientation { get { return (byte[,])OrientationProperty.Value; } }

        public LogProperty SmoothedRidgesProperty = new LogProperty("Extractor.RidgeSmoother")
        {
            Filter = options => options.UsesLayer(Options.Layer.SmoothedRidges)
        };
        public float[,] SmoothedRidges { get { return (float[,])SmoothedRidgesProperty.Value; } }

        public LogProperty OrthogonalSmoothingProperty = new LogProperty("Extractor.OrthogonalSmoother")
        {
            Filter = options => options.UsesLayer(Options.Layer.OrthogonalSmoothing)
        };
        public float[,] OrthogonalSmoothing { get { return (float[,])OrthogonalSmoothingProperty.Value; } }

        public LogProperty BinarizedProperty = new LogProperty("Extractor.Binarizer");
        public BinaryMap Binarized { get { return (BinaryMap)BinarizedProperty.Value; } }

        public LogProperty BinarySmoothingProperty = new LogProperty("Extractor.BinarySmoothingResult")
        {
            Filter = options => options.UsesLayer(Options.Layer.BinarySmoothing)
        };
        public BinaryMap BinarySmoothing { get { return (BinaryMap)BinarySmoothingProperty.Value; } }

        public LogProperty RemovedCrossesProperty = new LogProperty("Extractor.CrossRemover")
        {
            Filter = options => options.UsesLayer(Options.Layer.RemovedCrosses)
        };
        public BinaryMap RemovedCrosses { get { return (BinaryMap)RemovedCrossesProperty.Value; } }

        public LogProperty InnerMaskProperty = new LogProperty("Extractor.InnerMask")
        {
            Filter = options => options.Mask == Options.MaskType.Inner
        };
        public BinaryMap InnerMask { get { return (BinaryMap)InnerMaskProperty.Value; } }

        public LogProperty MinutiaCollectorProperty = new LogProperty("Extractor.MinutiaCollector");
        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)MinutiaCollectorProperty.Value; } }

        public ComputedProperty TemplateProperty = new ComputedProperty("MinutiaCollector");
        public Template Template { get { return new SerializedFormat().Export(MinutiaCollector); } }
    }
}
