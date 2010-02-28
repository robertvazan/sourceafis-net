using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SourceAFIS.Tuning;

namespace DatabaseAnalyzer
{
    sealed class MatcherReport : Report
    {
        public MatcherBenchmark Benchmark;

        public override void Create()
        {
            Path = "MatcherBenchmark.xml";
            CreateDocument("matcher-report");
            XmlElement prepare = AddChild("prepare");
            AddProperty(prepare, "count", Benchmark.Prepares.Count);
            AddProperty(prepare, "microseconds", Benchmark.Prepares.Milliseconds * 1000);
            CreateMatches(Benchmark.Matches, "matches");
            CreateMatches(Benchmark.NonMatches, "non-matches");
        }

        void CreateMatches(MatcherBenchmark.Statistics statistics, string name)
        {
            XmlElement matches = AddChild(name);
            AddProperty(matches, "count", statistics.Count);
            AddProperty(matches, "microseconds", statistics.Milliseconds * 1000);
        }
    }
}
