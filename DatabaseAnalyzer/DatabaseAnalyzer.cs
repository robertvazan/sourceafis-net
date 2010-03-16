using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Visualization;

namespace DatabaseAnalyzer
{
    sealed class DatabaseAnalyzer
    {
        Options Options = new Options();
        TestDatabase TestDatabase = new TestDatabase();
        ExtractorBenchmark ExtractorBenchmark = new ExtractorBenchmark();
        MatcherBenchmark MatcherBenchmark = new MatcherBenchmark();
        MatcherReport MatcherReport = new MatcherReport();

        public DatabaseAnalyzer()
        {
            Options.TestDatabase = TestDatabase;
            Options.ExtractorBenchmark = ExtractorBenchmark;
            ExtractorBenchmark.Database = TestDatabase;
            MatcherBenchmark.TestDatabase = TestDatabase;
            MatcherReport.Benchmark = MatcherBenchmark;
        }

        void Run()
        {
            Options.Load("DatabaseAnalyzerConfiguration.xml");
            switch (Options.Action)
            {
                case "extractor-benchmark":
                    RunExtractorBenchmark();
                    break;
                case "matcher-benchmark":
                    RunMatcherBenchmark();
                    break;
            }
        }

        void RunExtractorBenchmark()
        {
            Console.WriteLine("Running extractor benchmark");
            ExtractorReport report = ExtractorBenchmark.Run();
            Console.WriteLine("Saving extractor report");
            report.Save("Extractor");
            TestDatabase = report.Templates;
        }

        void RunMatcherBenchmark()
        {
            string dbPath = Path.Combine("Extractor", "Templates.dat");
            if (File.Exists(dbPath))
                TestDatabase.Load(dbPath);
            else
                RunExtractorBenchmark();
            Console.WriteLine("Running matcher benchmark");
            MatcherBenchmark.Run();
            
            MatcherReport.Create();
            MatcherReport.Save();

            ROCGraph graph = new ROCGraph();
            for (int i = 0; i < MatcherBenchmark.ROCs.Length; ++i)
                ImageIO.CreateBitmap(PixelFormat.ToColorB(graph.Draw(MatcherBenchmark.ROCs[i]))).Save("ROC" + (i + 1).ToString() + ".png");
        }

        static void Main(string[] args)
        {
            DatabaseAnalyzer instance = new DatabaseAnalyzer();
            instance.Run();
        }
    }
}
