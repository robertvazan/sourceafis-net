using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Tuning.Database;
#if !MONO
using SourceAFIS.Visualization;
#endif

namespace DatabaseAnalyzer
{
    sealed class DatabaseAnalyzer
    {
        Options Options = new Options();
        DatabaseCollection TestDatabase = new DatabaseCollection();
        ExtractorBenchmark ExtractorBenchmark = new ExtractorBenchmark();
        MatcherBenchmark MatcherBenchmark = new MatcherBenchmark();
        Optimizer Optimizer = new Optimizer();

        public DatabaseAnalyzer()
        {
            Options.TestDatabase = TestDatabase;
            Options.ExtractorBenchmark = ExtractorBenchmark;
            Options.MatcherBenchmark = MatcherBenchmark;
            Options.Optimizer = Optimizer;
            ExtractorBenchmark.Database = TestDatabase;
            MatcherBenchmark.TestDatabase = TestDatabase;
            Optimizer.ExtractorBenchmark = ExtractorBenchmark;
            Optimizer.MatcherBenchmark = MatcherBenchmark;
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
                case "optimizer":
                    RunOptimizer();
                    break;
            }
        }

        void RunExtractorBenchmark()
        {
            Console.WriteLine("Running extractor benchmark");
            ExtractorReport report = ExtractorBenchmark.Run();
            Console.WriteLine("Saving extractor report");
            report.Save("Extractor");
            MatcherBenchmark.TestDatabase = TestDatabase = report.Templates;
        }

        void RunMatcherBenchmark()
        {
#if !MONO
            if (Options.RenderGraph)
                AccuracyStatistics.GraphDrawer = (curve, file) => { WpfIO.Save(RocGraph.Render(curve), file); };
#endif

            string dbPath = Path.Combine("Extractor", "Templates.dat");
            if (File.Exists(dbPath))
                TestDatabase.Load(dbPath);
            else
                RunExtractorBenchmark();
            
            Console.WriteLine("Running matcher benchmark");
            MatcherReport report = MatcherBenchmark.Run();
            Console.WriteLine("Saving matcher report");
            report.Save("Matcher");
        }

        void RunOptimizer()
        {
            Optimizer.OnException += delegate(Exception e)
            {
                Console.WriteLine("Optimizer iteration failed: {0}", e.ToString());
            };
            Optimizer.Mutations.OnMutation += delegate(ParameterValue initial, ParameterValue mutated)
            {
                Console.WriteLine("Mutated {0}, {1} -> {2}", initial.FieldPath, initial.Value.Double, mutated.Value.Double);
            };
            Optimizer.NicheSlot.OnAccepted += message =>
            {
                Console.WriteLine("-----> Accepted: " + message);
                Optimizer.NicheSlot.Save("Optimizer");
            };
            Optimizer.NicheSlot.OnRejected += message =>
            {
                Console.WriteLine("Rejected: " + message);
            };
            Console.WriteLine("Running optimizer");
            Optimizer.Run();
        }

        [STAThread]
        static void Main(string[] args)
        {
            DatabaseAnalyzer instance = new DatabaseAnalyzer();
            instance.Run();
        }
    }
}
