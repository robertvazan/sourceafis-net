using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning
{
    public sealed class ROCCurve
    {
        public List<ROCPoint> Curve;

        struct Counts
        {
            public int Matching;
            public int NonMatching;
        }

        public void Compute(ScoreTable table)
        {
            Dictionary<float, Counts> countsByThreshold = AggregateByScore(table);
            Counts totals = SumTotals(countsByThreshold.Values);
            List<float> thresholds = new List<float>(countsByThreshold.Keys);
            thresholds.Sort();

            Curve = new List<ROCPoint>();
            Counts seenBefore = new Counts();
            foreach (float threshold in thresholds)
            {
                Counts counts = countsByThreshold[threshold];

                // false matches are non-matching pairs at or above threshold
                int falseMatches = totals.NonMatching - seenBefore.NonMatching;
                // false rejects are matching pairs below threshold
                int falseRejects = seenBefore.Matching;

                ROCPoint point;
                point.Threshold = threshold;
                point.FAR = falseMatches / (float)totals.NonMatching;
                point.FRR = falseRejects / (float)totals.Matching;
                Curve.Add(point);

                seenBefore.NonMatching += counts.NonMatching;
                seenBefore.Matching += counts.Matching;
            }

            ROCPoint final;
            final.Threshold = Curve[Curve.Count - 1].Threshold;
            final.FAR = 0;
            final.FRR = 1;
            Curve.Add(final);
        }

        Dictionary<float, Counts> AggregateByScore(ScoreTable table)
        {
            Dictionary<float, Counts> countsByThreshold = new Dictionary<float, Counts>();
            foreach (ScoreTable.Entry[] finger in table.Table)
                foreach (ScoreTable.Entry view in finger)
                {
                    foreach (float score in view.Matching)
                        UpdateCounts(countsByThreshold, score, 1, 0);

                    foreach (float score in view.NonMatching)
                        UpdateCounts(countsByThreshold, score, 0, 1);
                }
            return countsByThreshold;
        }

        void UpdateCounts(Dictionary<float, Counts> countsByThreshold, float score, int addMatching, int addNonMatching)
        {
            Counts counts = new Counts();
            if (countsByThreshold.ContainsKey(score))
                counts = countsByThreshold[score];
            counts.Matching += addMatching;
            counts.NonMatching += addNonMatching;
            countsByThreshold[score] = counts;
        }

        Counts SumTotals(IEnumerable<Counts> list)
        {
            Counts totals = new Counts();
            foreach (Counts counts in list)
            {
                totals.Matching += counts.Matching;
                totals.NonMatching += counts.NonMatching;
            }
            return totals;
        }
    }
}
