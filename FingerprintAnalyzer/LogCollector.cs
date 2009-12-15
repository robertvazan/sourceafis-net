using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.General;

namespace FingerprintAnalyzer
{
    class LogCollector
    {
        public struct ExtractionData
        {
            public byte[,] InputImage;
            public BlockMap Blocks;
            public byte[,] BlockContrast;
            public BinaryMap AbsoluteContrast;
            public float[,] Equalized;
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
            Probe.BlockContrast = Logger.Retrieve<byte[,]>("Extractor.Contrast");
            Probe.AbsoluteContrast = Logger.Retrieve<BinaryMap>("Extractor.AbsoluteContrast");
            Probe.Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
            Logger.Clear();
        }
    }
}
