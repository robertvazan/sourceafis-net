using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.Meta;
using SourceAFIS.Tuning.Optimization;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Tuning
{
    public sealed class Optimizer
    {
        public ExtractorBenchmark ExtractorBenchmark = new ExtractorBenchmark();
        public MatcherBenchmark MatcherBenchmark = new MatcherBenchmark();
        public NicheSlot NicheSlot = new NicheSlot();
        public MutationSequencer Mutations = new MutationSequencer();

        public Action<Exception> OnException;

        public Optimizer()
        {
            NicheSlot.Measure.ErrorPolicyFunction = ErrorPolicy.ZeroFAR;
            NicheSlot.Measure.ScalarMeasure = ScalarErrorMeasure.FRR;
        }

        public void Run()
        {
            SetTimeouts();
            ParameterSet previous = null;
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

                bool improved = false;
                try
                {
                    report.Extractor = NicheSlot.GetCachedTemplates(trial);
                    if (report.Extractor == null)
                        report.Extractor = ExtractorBenchmark.Run();
                    report.Configuration.DatabaseStatistics.Collect(report.Extractor.Templates);
                    MatcherBenchmark.TestDatabase = report.Extractor.Templates;

                    report.Matcher = MatcherBenchmark.Run();

                    improved = NicheSlot.Fit(report);
                }
                catch (Exception e)
                {
                    if (OnException != null)
                        OnException(e);
                }

                if (NicheSlot.BestSolution == null)
                    throw new ApplicationException("Initial parameter set doesn't meet basic NicheSlot criteria");
                if (previous != null)
                    Mutations.Feedback(previous, trial, improved);
                previous = NicheSlot.BestSolution.Configuration.Parameters;
                trial = Mutations.Mutate(previous);
            }
        }

        void SetTimeouts()
        {
            ExtractorBenchmark.Timeout = NicheSlot.TimeConstraints.Extraction * ExtractorBenchmark.Database.FpCount + 10;
            MatcherBenchmark.Timeout = NicheSlot.TimeConstraints.Prepare * ExtractorBenchmark.Database.FpCount +
                NicheSlot.TimeConstraints.Matching * ExtractorBenchmark.Database.GetMatchingPairCount() +
                NicheSlot.TimeConstraints.NonMatching * ExtractorBenchmark.Database.GetNonMatchingPairCount() + 10;
        }
    }
}
