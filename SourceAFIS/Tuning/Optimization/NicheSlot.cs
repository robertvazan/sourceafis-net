using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
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

        public delegate void ChangeEvent();
        public ChangeEvent OnChange;

        public bool Fit(TestReport solution)
        {
            if (TimeConstraints.Check(solution.Extractor) && TimeConstraints.Check(solution.Matcher))
            {
                AccuracyStatistics performance = new AccuracyStatistics();
                performance.Compute(solution.Matcher.ScoreTables, Measure);

                if (BestSolution == null || BestPerformance.Average > performance.Average)
                {
                    bool improved = BestSolution != null;

                    BestSolution = solution;
                    BestPerformance = performance;

                    if (OnChange != null)
                        OnChange();

                    return improved;
                }
            }
            return false;
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
