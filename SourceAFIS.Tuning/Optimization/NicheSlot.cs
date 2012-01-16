using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Tuning.Reports;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class NicheSlot
    {
        public string Name = "NicheSlot";
        public TimeConstraints TimeConstraints = new TimeConstraints();
        public AccuracyMeasure Measure = new AccuracyMeasure();

        public TestReport BestSolution;
        public AccuracyStatistics BestPerformance;

        public Action<string> OnAccepted;
        public Action<string> OnRejected;

        public bool Fit(TestReport solution)
        {
            try
            {
                TimeConstraints.Check(solution.Extractor);
                TimeConstraints.Check(solution.Matcher);

                AccuracyStatistics performance = new AccuracyStatistics(solution.Matcher.ScoreTables, Measure);
                string acceptance;

                if (BestSolution != null)
                {
                    if (performance.AverageError > BestPerformance.AverageError)
                        throw new FailedMutationException("Worse accuracy: {0:P2} -> {1:P2}", BestPerformance.AverageError, performance.AverageError);
                    else if (performance.AverageError == BestPerformance.AverageError)
                    {
                        if (performance.Separation > BestPerformance.Separation)
                            throw new FailedMutationException("Same accuracy, worse separation: {0:F4} -> {1:F4}",
                                BestPerformance.Separation, performance.Separation);
                        else if (performance.Separation == BestPerformance.Separation)
                        {
                            if (solution.Matcher.Time.NonMatching > BestSolution.Matcher.Time.NonMatching)
                                throw new FailedMutationException("Same accuracy, same separation, worse speed: {0:F0} fp/s -> {1:F0} fp/s",
                                    1 / BestSolution.Matcher.Time.NonMatching, 1 / solution.Matcher.Time.NonMatching);
                            else if (solution.Matcher.Time.NonMatching > BestSolution.Matcher.Time.NonMatching)
                                throw new FailedMutationException("Same accuracy, same separation, same performance");
                            else
                                acceptance = String.Format("Better speed: {0:F0} fp/s -> {1:F0} fp/s",
                                    1 / BestSolution.Matcher.Time.NonMatching, 1 / solution.Matcher.Time.NonMatching);
                        }
                        else
                            acceptance = String.Format("Better separation: {0:F4} -> {1:F4}", BestPerformance.Separation, performance.Separation);
                    }
                    else
                        acceptance = String.Format("Better accuracy: {0:P2} -> {1:P2}", BestPerformance.AverageError, performance.AverageError);
                }
                else
                    acceptance = "Initial";

                bool improved = BestSolution != null && (performance.AverageError < BestPerformance.AverageError
                    || performance.Separation > BestPerformance.Separation);

                BestSolution = solution;
                BestPerformance = performance;

                if (OnAccepted != null)
                    OnAccepted(acceptance);

                return improved;
            }
            catch (FailedMutationException e)
            {
                if (OnRejected != null)
                    OnRejected(e.Message);
                return false;
            }
        }

        public void Save(string folder)
        {
            BestSolution.Save(folder);
            BestPerformance.Save(folder, false);
        }

        public ExtractorReport GetCachedTemplates(ParameterSet query)
        {
            if (BestSolution != null)
            {
                ParameterSet filteredQuery = query.GetSubset("Extractor.");
                ParameterSet filteredCache = BestSolution.Configuration.Parameters.GetSubset("Extractor.");
                if (filteredCache.PersistentlyEquals(filteredQuery))
                    return BestSolution.Extractor;
            }
            return null;
        }
    }
}
