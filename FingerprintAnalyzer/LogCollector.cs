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
            Extractor.Extract(Probe.InputImage);
            Probe.Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
            Logger.Clear();
        }
    }
}
