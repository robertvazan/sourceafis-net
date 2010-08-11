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
                return (GetMatchingMin(table) - GetNonMatchingMax(table)) / GetMedianDistance(table);
            }

            float GetMatchingMin(ScoreTable table)
            {
                float min = Single.MaxValue;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).Matching)
                        min = Math.Min(min, score);
                return min;
            }

            float GetNonMatchingMax(ScoreTable table)
            {
                float max = Single.MinValue;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).NonMatching)
                        max = Math.Max(max, score);
                return max;
            }
        }

        public static readonly SeparationMeasure MinMax = new MinMaxMeasure();

        sealed class StandardDeviationMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                float distance = GetAveragesDistance(table);
                float matchingAverage = GetMatchingAverage(table);
                float matching = AverageMatching(table, score => Calc.Sq(score - matchingAverage));
                float nonmatchingAverage = GetNonMatchingAverage(table);
                float nonmatching = AverageNonMatching(table, score => Calc.Sq(score - nonmatchingAverage));
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
                float matching = AverageMatching(table,
                    score => score < matchingMedian ? Calc.Sq(score - matchingMedian) : 0);
                float nonmatchingMedian = GetNonMatchingMedian(table);
                float nonmatching = AverageNonMatching(table,
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
                float matching = AverageMatching(table,
                    score => score < matchingMedian ? matchingMedian - score : 0);
                float nonmatchingMedian = GetNonMatchingMedian(table);
                float nonmatching = AverageNonMatching(table,
                    score => score > nonmatchingMedian ? score - nonmatchingMedian : 0);
                return (distance - matching - nonmatching) / distance;
            }
        }

        public static readonly SeparationMeasure HalfDistance = new HalfDistanceMeasure();

        protected delegate float ScoreTransform(float score);

        protected float AverageMatching(ScoreTable table, ScoreTransform transform)
        {
            double sum = 0;
            int count = 0;
            foreach (ScoreTable.Index index in table.GetAllIndexes())
            {
                float[] scores = table.GetEntry(index).Matching;
                count += scores.Length;
                sum += scores.Sum(score => transform(score));
            }
            return (float)(sum / count);
        }

        protected float AverageNonMatching(ScoreTable table, ScoreTransform transform)
        {
            double sum = 0;
            int count = 0;
            foreach (ScoreTable.Index index in table.GetAllIndexes())
            {
                float[] scores = table.GetEntry(index).NonMatching;
                count += scores.Length;
                sum += scores.Sum(score => transform(score));
            }
            return (float)(sum / count);
        }

        protected float GetMedianDistance(ScoreTable table)
        {
            return GetMatchingMedian(table) - GetNonMatchingMedian(table);
        }

        protected float GetMatchingMedian(ScoreTable table)
        {
            List<float> all = new List<float>();
            foreach (ScoreTable.Index index in table.GetAllIndexes())
                all.AddRange(table.GetEntry(index).Matching);
            all.Sort();
            return all[all.Count / 2];
        }

        protected float GetNonMatchingMedian(ScoreTable table)
        {
            List<float> all = new List<float>();
            foreach (ScoreTable.Index index in table.GetAllIndexes())
                all.AddRange(table.GetEntry(index).NonMatching);
            all.Sort();
            return all[all.Count / 2];
        }

        protected float GetAveragesDistance(ScoreTable table)
        {
            return GetMatchingAverage(table) - GetNonMatchingAverage(table);
        }

        protected float GetMatchingAverage(ScoreTable table)
        {
            double sum = 0;
            int count = 0;
            foreach (ScoreTable.Index index in table.GetAllIndexes())
            {
                float[] scores = table.GetEntry(index).Matching;
                count += scores.Length;
                sum += scores.Sum();
            }
            return (float)(sum / count);
        }

        protected float GetNonMatchingAverage(ScoreTable table)
        {
            double sum = 0;
            int count = 0;
            foreach (ScoreTable.Index index in table.GetAllIndexes())
            {
                float[] scores = table.GetEntry(index).NonMatching;
                count += scores.Length;
                sum += scores.Sum();
            }
            return (float)(sum / count);
        }
    }
}
