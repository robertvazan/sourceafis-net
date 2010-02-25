using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using SourceAFIS.Tuning;

namespace DatabaseAnalyzer
{
    sealed class Options
    {
        public TestDatabase TestDatabase;
        public ExtractorBenchmark ExtractorBenchmark;

        public string Action;

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

            ExtractorBenchmark.MaxTotalSeconds = GetInt("extractor-benchmark/max-seconds", ExtractorBenchmark.MaxTotalSeconds);

            Action = Root.SelectSingleNode("/database-analyzer/action").Value;
        }

        delegate void ClipFunction(int max);

        void ClipDatabase(string name, ClipFunction function)
        {
            XPathNavigator limit = Root.SelectSingleNode("/database-analyzer/test-database/" + name);
            if (limit != null)
                function(Convert.ToInt32(limit.Value));
        }

        int GetInt(string path, int defaultValue)
        {
            XPathNavigator element = Root.SelectSingleNode("/database-analyzer/" + path);
            if (element != null)
                return Convert.ToInt32(element.Value);
            else
                return defaultValue;
        }
    }
}
