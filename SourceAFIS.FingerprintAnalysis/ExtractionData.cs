using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ExtractionData : LogData
    {
        public override LogCollector Collector
        {
            get { return base.Collector; }
            set
            {
                base.Collector = value;
                Ridges.Collector = value;
                Valleys.Collector = value;
            }
        }

        public SkeletonData Ridges = new SkeletonData("[Ridges]");
        public SkeletonData Valleys = new SkeletonData("[Valleys]");

        public ExtractionCollector ExtractionCollector { get { return Collector as ExtractionCollector; } }

        public byte[,] InputImage { get { Link(ExtractionCollector, "InputImage", "InputImage"); return ExtractionCollector.InputImage; } }

        public double Width { get { Link("InputImage", "Width"); byte[,] image = InputImage; return image != null ? image.GetLength(1) : 480; } }

        public double Height { get { Link("InputImage", "Height"); byte[,] image = InputImage; return image != null ? image.GetLength(0) : 640; } }

        public BlockMap Blocks { get { return (BlockMap)GetLog("Blocks", "BlockMap"); } }

        public byte[,] BlockContrast { get { return (byte[,])GetLog("BlockContrast", "Mask.Contrast"); } }

        public BinaryMap AbsoluteContrast { get { return (BinaryMap)GetLog("AbsoluteContrast", "Mask.AbsoluteContrast"); } }

        public BinaryMap RelativeContrast { get { return (BinaryMap)GetLog("RelativeContrast", "Mask.RelativeContrast"); } }

        public BinaryMap LowContrastMajority { get { return (BinaryMap)GetLog("LowContrastMajority", "Mask.LowContrastMajority"); } }

        public BinaryMap SegmentationMask { get { return (BinaryMap)GetLog("SegmentationMask", "Mask"); } }

        public float[,] Equalized { get { return (float[,])GetLog("Equalized", "Equalizer"); } }

        public byte[,] Orientation { get { return (byte[,])GetLog("Orientation", "Orientation"); } }

        public float[,] SmoothedRidges { get { return (float[,])GetLog("SmoothedRidges", "RidgeSmoother"); } }

        public float[,] OrthogonalSmoothing { get { return (float[,])GetLog("OrthogonalSmoothing", "OrthogonalSmoother"); } }

        public BinaryMap Binarized { get { return (BinaryMap)GetLog("Binarized", "Binarizer"); } }

        public BinaryMap BinarySmoothing { get { return (BinaryMap)GetLog("BinarySmoothing", "BinarySmoothingResult"); } }

        public BinaryMap RemovedCrosses { get { return (BinaryMap)GetLog("RemovedCrosses", "CrossRemover"); } }

        public BinaryMap InnerMask { get { return (BinaryMap)GetLog("InnerMask", "InnerMask"); } }

        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)GetLog("MinutiaCollector", "MinutiaCollector", 1); } }

        public TemplateBuilder MinutiaMask { get { return (TemplateBuilder)GetLog("MinutiaMask", "MinutiaMask"); } }

        public TemplateBuilder MinutiaCloudRemover { get { return (TemplateBuilder)GetLog("MinutiaCloudRemover", "MinutiaCloudRemover"); } }

        public TemplateBuilder UniqueMinutiaSorter { get { return (TemplateBuilder)GetLog("UniqueMinutiaSorter", "UniqueMinutiaSorter"); } }

        public TemplateBuilder FinalTemplate { get { return (TemplateBuilder)GetLog("FinalTemplate", "FinalTemplate"); } }

        public Template Template
        {
            get
            {
                Link("FinalTemplate", "Template");
                return FinalTemplate != null ? new SerializedFormat().Export(FinalTemplate) : null;
            }
        }
    }
}
