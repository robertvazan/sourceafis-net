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

        public delegate void ExceptionEvent(Exception e);
        public ExceptionEvent OnException;

        public void Run()
        {
            SetTimeouts();
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

                    NicheSlot.Fit(report);
                }
                catch (Exception e)
                {
                    if (OnException != null)
                        OnException(e);
                }

                if (NicheSlot.BestSolution == null)
                    throw new Exception();
                trial = Mutations.Mutate(NicheSlot.BestSolution.Configuration.Parameters);
            }
        }

        void SetTimeouts()
        {
            ExtractorBenchmark.Timeout = NicheSlot.TimeConstraints.Extraction * ExtractorBenchmark.Database.GetFingerprintCount();
            MatcherBenchmark.Timeout = NicheSlot.TimeConstraints.Prepare * ExtractorBenchmark.Database.GetFingerprintCount() +
                NicheSlot.TimeConstraints.Matching * ExtractorBenchmark.Database.GetMatchingPairCount() +
                NicheSlot.TimeConstraints.NonMatching * ExtractorBenchmark.Database.GetNonMatchingPairCount();
        }
    }
}
