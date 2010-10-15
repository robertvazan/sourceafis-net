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

        public BlockMap Blocks { get { return (BlockMap)GetLog("Blocks", "Extractor.BlockMap"); } }

        public byte[,] BlockContrast { get { return (byte[,])GetLog("BlockContrast", "Extractor.Mask.Contrast"); } }

        public BinaryMap AbsoluteContrast { get { return (BinaryMap)GetLog("AbsoluteContrast", "Extractor.Mask.AbsoluteContrast"); } }

        public BinaryMap RelativeContrast { get { return (BinaryMap)GetLog("RelativeContrast", "Extractor.Mask.RelativeContrast"); } }

        public BinaryMap LowContrastMajority { get { return (BinaryMap)GetLog("LowContrastMajority", "Extractor.Mask.LowContrastMajority"); } }

        public BinaryMap SegmentationMask { get { return (BinaryMap)GetLog("SegmentationMask", "Extractor.Mask"); } }

        public float[,] Equalized { get { return (float[,])GetLog("Equalized", "Extractor.Equalizer"); } }

        public byte[,] Orientation { get { return (byte[,])GetLog("Orientation", "Extractor.Orientation"); } }

        public float[,] SmoothedRidges { get { return (float[,])GetLog("SmoothedRidges", "Extractor.RidgeSmoother"); } }

        public float[,] OrthogonalSmoothing { get { return (float[,])GetLog("OrthogonalSmoothing", "Extractor.OrthogonalSmoother"); } }

        public BinaryMap Binarized { get { return (BinaryMap)GetLog("Binarized", "Extractor.Binarizer"); } }

        public BinaryMap BinarySmoothing { get { return (BinaryMap)GetLog("BinarySmoothing", "Extractor.BinarySmoothingResult"); } }

        public BinaryMap RemovedCrosses { get { return (BinaryMap)GetLog("RemovedCrosses", "Extractor.CrossRemover"); } }

        public BinaryMap InnerMask { get { return (BinaryMap)GetLog("InnerMask", "Extractor.InnerMask"); } }

        public TemplateBuilder MinutiaCollector { get { return (TemplateBuilder)GetLog("MinutiaCollector", "Extractor.MinutiaCollector"); } }

        public Template Template { get { Watch("MinutiaCollector", "Template"); return new SerializedFormat().Export(MinutiaCollector); } }
    }
}
