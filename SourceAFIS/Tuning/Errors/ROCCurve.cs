using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SourceAFIS.Tuning.Errors
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
            var matching = from score in table.Matching
                           select new { Score = score, Match = true };
            var nonmatching = from score in table.NonMatching
                              select new { Score = score, Match = false };
            var counted = from duplicate in matching.Concat(nonmatching)
                          group duplicate by duplicate.Score into grouped
                          select new
                          {
                              Score = grouped.Key,
                              Matching = grouped.Count(groupedDuplicate => groupedDuplicate.Match),
                              NonMatching = grouped.Count(groupedDuplicate => !groupedDuplicate.Match)
                          };
            return counted.ToDictionary(unique => unique.Score,
                unique => new Counts { Matching = unique.Matching, NonMatching = unique.NonMatching });
        }

        Counts SumTotals(IEnumerable<Counts> list)
        {
            return new Counts
            {
                Matching = list.Sum(counts => counts.Matching),
                NonMatching = list.Sum(counts => counts.NonMatching)
            };
        }
    }
}
