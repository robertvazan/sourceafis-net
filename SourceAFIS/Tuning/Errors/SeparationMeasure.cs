using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Errors
{
    public abstract class SeparationMeasure
    {
        public abstract float Measure(ScoreTable table);

        sealed class MinMaxMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                return (table.Matching.Min() - table.NonMatching.Max()) / GetMedianDistance(table);
            }
        }

        public static readonly SeparationMeasure MinMax = new MinMaxMeasure();

        sealed class StandardDeviationMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                float distance = GetAveragesDistance(table);
                float matchingAverage = table.Matching.Average();
                float matching = table.Matching.Average(score => Calc.Sq(score - matchingAverage));
                float nonmatchingAverage = table.NonMatching.Average();
                float nonmatching = table.NonMatching.Average(score => Calc.Sq(score - nonmatchingAverage));
                return (distance - (float)Math.Sqrt(matching) - (float)Math.Sqrt(nonmatching)) / distance;
            }
        }

        public static readonly SeparationMeasure StandardDeviation = new StandardDeviationMeasure();

        sealed class HalfDeviationMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                float distance = GetMedianDistance(table);
                float matchingMedian = GetMatchingMedian(table);
                float matching = table.Matching.Average(score => score < matchingMedian ? Calc.Sq(score - matchingMedian) : 0);
                float nonmatchingMedian = GetNonMatchingMedian(table);
                float nonmatching = table.NonMatching.Average(
                    score => score > nonmatchingMedian ? Calc.Sq(score - nonmatchingMedian) : 0);
                return (distance - (float)Math.Sqrt(matching) - (float)Math.Sqrt(nonmatching)) / distance;
            }
        }

        public static readonly SeparationMeasure HalfDeviation = new HalfDeviationMeasure();

        sealed class HalfDistanceMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                float distance = GetMedianDistance(table);
                float matchingMedian = GetMatchingMedian(table);
                float matching = table.Matching.Average(score => score < matchingMedian ? matchingMedian - score : 0);
                float nonmatchingMedian = GetNonMatchingMedian(table);
                float nonmatching = table.NonMatching.Average(score => score > nonmatchingMedian ? score - nonmatchingMedian : 0);
                return (distance - matching - nonmatching) / distance;
            }
        }

        public static readonly SeparationMeasure HalfDistance = new HalfDistanceMeasure();

        protected float GetMedianDistance(ScoreTable table)
        {
            return GetMatchingMedian(table) - GetNonMatchingMedian(table);
        }

        protected float GetMatchingMedian(ScoreTable table)
        {
            List<float> all = new List<float>(table.Matching);
            all.Sort();
            return all[all.Count / 2];
        }

        protected float GetNonMatchingMedian(ScoreTable table)
        {
            List<float> all = new List<float>(table.NonMatching);
            all.Sort();
            return all[all.Count / 2];
        }

        protected float GetAveragesDistance(ScoreTable table)
        {
            return table.Matching.Average() - table.NonMatching.Average();
        }
    }
}
