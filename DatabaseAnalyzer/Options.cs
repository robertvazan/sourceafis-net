using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Database;

namespace DatabaseAnalyzer
{
    sealed class Options
    {
        public DatabaseCollection TestDatabase;
        public ExtractorBenchmark ExtractorBenchmark;
        public MatcherBenchmark MatcherBenchmark;
        public Optimizer Optimizer;

        public string Action;
        public bool RenderGraph;

        XPathDocument XmlDocument;
        XPathNavigator Root;

        public void Load(string path)
        {
            XmlDocument = new XPathDocument(path);
            Root = XmlDocument.CreateNavigator();

            foreach (XPathNavigator element in Root.Select("/database-analyzer/test-database/scan"))
            {
                Console.WriteLine("Scanning folder {0}", element.Value);
                TestDatabase.Scan(element.Value);
            }
            ClipDatabase("database-count", TestDatabase.ClipDatabaseCount);
            ClipDatabase("fingers-per-database", TestDatabase.ClipFingersPerDatabase);
            ClipDatabase("views-per-finger", TestDatabase.ClipViewsPerFinger);

            ExtractorBenchmark.Timeout = GetFloat("extractor-benchmark/max-seconds", ExtractorBenchmark.Timeout);

            MatcherBenchmark.MaxMatchingPerProbe = GetInt("matcher-benchmark/max-matching-pairs", Int32.MaxValue);
            MatcherBenchmark.MaxNonMatchingPerProbe = GetInt("matcher-benchmark/max-non-matching-pairs", Int32.MaxValue);
            RenderGraph = GetBoolean("matcher-benchmark/render-graph", true);

            foreach (XPathNavigator element in Root.Select("/database-analyzer/optimizer/mutate"))
                Optimizer.Mutations.ManualAdvisor.ParameterPaths.Add(element.Value);

            Action = Root.SelectSingleNode("/database-analyzer/action").Value;
        }

        void ClipDatabase(string name, Action<int> clipMethod)
        {
            XPathNavigator limit = Root.SelectSingleNode("/database-analyzer/test-database/" + name);
            if (limit != null)
                clipMethod(Convert.ToInt32(limit.Value));
        }

        int GetInt(string path, int defaultValue)
        {
            XPathNavigator element = Root.SelectSingleNode("/database-analyzer/" + path);
            if (element != null)
                return Convert.ToInt32(element.Value);
            else
                return defaultValue;
        }

        float GetFloat(string path, float defaultValue)
        {
            XPathNavigator element = Root.SelectSingleNode("/database-analyzer/" + path);
            if (element != null)
                return Convert.ToSingle(element.Value);
            else
                return defaultValue;
        }

        bool GetBoolean(string path, bool defaultValue)
        {
            XPathNavigator element = Root.SelectSingleNode("/database-analyzer/" + path);
            if (element != null)
                return Convert.ToBoolean(element.Value);
            else
                return defaultValue;
        }
    }
}
