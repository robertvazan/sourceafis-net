using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using SourceAFIS.Tuning;

namespace DatabaseAnalyzer
{
    sealed class DatabaseAnalyzer
    {
        Options Options = new Options();
        TestDatabase TestDatabase = new TestDatabase();
        ExtractorBenchmark ExtractorBenchmark = new ExtractorBenchmark();
        ExtractorReport ExtractorReport = new ExtractorReport();
        MatcherBenchmark MatcherBenchmark = new MatcherBenchmark();
        MatcherReport MatcherReport = new MatcherReport();

        public DatabaseAnalyzer()
        {
            Options.TestDatabase = TestDatabase;
            Options.ExtractorBenchmark = ExtractorBenchmark;
            ExtractorBenchmark.Database = TestDatabase;
            ExtractorReport.Benchmark = ExtractorBenchmark;
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
            ExtractorBenchmark.Run();
            ExtractorReport.Create();
            ExtractorReport.Save();
            TestDatabase.Save("TestDatabase.dat");
        }

        void RunMatcherBenchmark()
        {
            if (File.Exists("TestDatabase.dat"))
                TestDatabase.Load("TestDatabase.dat");
            else
            {
                Console.WriteLine("Running extraction");
                ExtractorBenchmark.Run();
            }
            Console.WriteLine("Running matcher benchmark");
            MatcherBenchmark.Run();
            MatcherReport.Create();
            MatcherReport.Save();
        }

        static void Main(string[] args)
        {
            DatabaseAnalyzer instance = new DatabaseAnalyzer();
            instance.Run();
        }
    }
}
