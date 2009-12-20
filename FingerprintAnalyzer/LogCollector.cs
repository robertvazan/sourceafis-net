using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.General;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class LogCollector
    {
        public struct ExtractionData
        {
            public byte[,] InputImage;
            public BlockMap Blocks;
            public byte[,] BlockContrast;
            public BinaryMap AbsoluteContrast;
            public BinaryMap RelativeContrast;
            public BinaryMap LowContrastMajority;
            public BinaryMap SegmentationMask;
            public float[,] Equalized;
            public byte[,] Orientation;
            public float[,] SmoothedRidges;
        }

        public ExtractionData Probe;

        Extractor Extractor = new Extractor();

        public LogCollector()
        {
            Logger.Resolver.Scan(Extractor, "Extractor");
            Logger.Filter = delegate(string path) { return true; };
        }

        public void Collect()
        {
            Extractor.Extract(Probe.InputImage, 500);
            Probe.Blocks = Logger.Retrieve<BlockMap>("Extractor.BlockMap");
            Probe.BlockContrast = Logger.Retrieve<byte[,]>("Extractor.Mask.Contrast");
            Probe.AbsoluteContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.AbsoluteContrast");
            Probe.RelativeContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.RelativeContrast");
            Probe.LowContrastMajority = Logger.Retrieve<BinaryMap>("Extractor.Mask.LowContrastMajority");
            Probe.SegmentationMask = Logger.Retrieve<BinaryMap>("Extractor.Mask");
            Probe.Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
            Probe.Orientation = Logger.Retrieve<byte[,]>("Extractor.Orientation");
            Probe.SmoothedRidges = Logger.Retrieve<float[,]>("Extractor.RidgeSmoother");
            Logger.Clear();
        }
    }
}
