using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SourceAFIS.Tuning;

namespace DatabaseAnalyzer
{
    sealed class ExtractorReport : Report
    {
        public ExtractorBenchmark Benchmark;

        public override void Create()
        {
            Path = "ExtractorBenchmark.xml";
            CreateDocument("extractor-report");
            AddProperty("count", Benchmark.Count);
            AddProperty("milliseconds", 1000 * Benchmark.Average.Seconds);
            AddProperty("minutiae", Benchmark.Average.Minutiae);
            AddProperty("bytes", Benchmark.Average.TemplateBytes);
        }
    }
}
