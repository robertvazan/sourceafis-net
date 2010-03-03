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
            AddProperty(prepare, "milliseconds", Benchmark.Prepares.Milliseconds, 3);
            CreateMatches(Benchmark.Matches, "matches");
            CreateMatches(Benchmark.NonMatches, "non-matches");
            
            XmlElement table = AddChild("database-table");
            for (int i = 0; i < Benchmark.TestDatabase.Databases.Count; ++i)
                CreateMultiFinger(table, Benchmark.TestDatabase.Databases[i].Path, Benchmark.PerDatabaseErrors[i]);
            CreateMultiFinger(table, "average", Benchmark.AverageErrors);
        }

        void CreateMatches(MatcherBenchmark.Statistics statistics, string name)
        {
            XmlElement matches = AddChild(name);
            AddProperty(matches, "count", statistics.Count);
            AddProperty(matches, "milliseconds", statistics.Milliseconds, 3);
        }

        void CreateMultiFinger(XmlElement parent, string name, MultiFingerStatistics statistics)
        {
            XmlElement element = AddChild(parent, "multi-finger", name);
            CreateErrorStatistics(element, "simple", statistics.Simple);
            CreateErrorStatistics(element, "1-of-2", statistics.Take1Of2);
            CreateErrorStatistics(element, "2-of-3", statistics.Take2Of3);
            CreateErrorStatistics(element, "2-of-4", statistics.Take2Of4);
            CreateErrorStatistics(element, "3-of-5", statistics.Take3Of5);
        }

        void CreateErrorStatistics(XmlElement parent, string name, ErrorStatistics statistics)
        {
            XmlElement element = AddChild(parent, "error-statistics", name);
            CreateErrorRate(element, "eer", statistics.EER);
            CreateErrorRate(element, "prefer-far", statistics.PreferFAR);
            CreateErrorRate(element, "far100", statistics.FAR100);
            CreateErrorRate(element, "zero-far", statistics.ZeroFAR);
        }

        void CreateErrorRate(XmlElement parent, string name, ErrorRate rate)
        {
            XmlElement element = AddChild(parent, "error-rate", name);
            CreateROCPoint(element, "rate", rate.Rate);
            CreateROCPoint(element, "min", rate.Min);
            CreateROCPoint(element, "max", rate.Max);
        }

        void CreateROCPoint(XmlElement parent, string name, ROCPoint point)
        {
            XmlElement roc = AddChild(parent, "roc-point", name);
            roc.SetAttribute("far", FormatPercent(point.FAR, 2));
            roc.SetAttribute("frr", FormatPercent(point.FRR, 2));
            roc.SetAttribute("threshold", point.Threshold.ToString("F3"));
        }
    }
}
