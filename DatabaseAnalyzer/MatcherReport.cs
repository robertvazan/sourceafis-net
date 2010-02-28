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
            {
                XmlElement database = AddChild(table, "database");
                AddProperty(database, "eer", Benchmark.PerDatabaseEER[i] * 100, 3);
            }
            AddProperty("eer", Benchmark.EER * 100, 3);
        }

        void CreateMatches(MatcherBenchmark.Statistics statistics, string name)
        {
            XmlElement matches = AddChild(name);
            AddProperty(matches, "count", statistics.Count);
            AddProperty(matches, "milliseconds", statistics.Milliseconds, 3);
        }
    }
}
