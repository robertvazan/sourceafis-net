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
        public SkeletonData Ridges = new SkeletonData("[Ridges]");
        public SkeletonData Valleys = new SkeletonData("[Valleys]");

        public byte[,] InputImage { get; set; }

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

        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)GetLog("MinutiaCollector", "MinutiaCollector"); } }

        public Template Template { get { Watch("MinutiaCollector", "Template"); return new SerializedFormat().Export(MinutiaCollector); } }
    }
}
