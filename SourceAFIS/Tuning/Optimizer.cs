using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.Tuning.Optimization;
using SourceAFIS.Tuning.Reports;

namespace SourceAFIS.Tuning
{
    public sealed class Optimizer
    {
        public ExtractorBenchmark ExtractorBenchmark = new ExtractorBenchmark();
        public MatcherBenchmark MatcherBenchmark = new MatcherBenchmark();
        public NicheSlot NicheSlot = new NicheSlot();
        public MutationSequencer Mutations = new MutationSequencer();

        public void Run()
        {
            ParameterSet trial = new ParameterSet();
            trial.Add(new ObjectTree(ExtractorBenchmark.Extractor, "Extractor"));
            trial.Add(new ObjectTree(MatcherBenchmark.Matcher, "Matcher"));

            while (true)
            {
                trial.Rebind(new ObjectTree(ExtractorBenchmark.Extractor, "Extractor"));
                trial.Rebind(new ObjectTree(MatcherBenchmark.Matcher, "Matcher"));
                trial.SaveValues();

                TestReport report = new TestReport();
                report.Configuration.Parameters = trial.Clone();

                try
                {
                    report.Extractor = NicheSlot.GetCachedTemplates(trial);
                    if (report.Extractor == null)
                        report.Extractor = ExtractorBenchmark.Run();
                    MatcherBenchmark.TestDatabase = report.Extractor.Templates;

                    report.Matcher = MatcherBenchmark.Run();
                }
                catch (Exception)
                {
                }

                NicheSlot.Fit(report);

                if (NicheSlot.BestSolution == null)
                    throw new Exception();
                trial = Mutations.Mutate(NicheSlot.BestSolution.Configuration.Parameters);
            }
        }
    }
}
