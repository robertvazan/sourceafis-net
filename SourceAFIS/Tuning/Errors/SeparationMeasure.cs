using System;
using System.Collections.Generic;
using System.Text;
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
                float max = Single.MaxValue;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).NonMatching)
                        max = Math.Min(max, score);
                return max;
            }
        }

        public static readonly SeparationMeasure MinMax = new MinMaxMeasure();

        sealed class StandardDeviationMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                return (GetMatchingDeviation(table, GetMatchingAverage(table)) +
                    GetNonMatchingDeviation(table, GetNonMatchingAverage(table))) / GetAveragesDistance(table);
            }

            float GetMatchingDeviation(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).Matching)
                        sum += Calc.Sq(score - center);
                return (float)Math.Sqrt(sum);
            }

            float GetNonMatchingDeviation(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).NonMatching)
                        sum += Calc.Sq(score - center);
                return (float)Math.Sqrt(sum);
            }
        }

        public static readonly SeparationMeasure StandardDeviation = new StandardDeviationMeasure();

        sealed class HalfDeviationMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                return (GetMatchingDeviation(table, GetMatchingMedian(table)) +
                    GetNonMatchingDeviation(table, GetNonMatchingMedian(table))) / GetMedianDistance(table);
            }

            float GetMatchingDeviation(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).Matching)
                        if (score < center)
                            sum += Calc.Sq(score - center);
                return (float)Math.Sqrt(sum);
            }

            float GetNonMatchingDeviation(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).NonMatching)
                        if (score > center)
                            sum += Calc.Sq(score - center);
                return (float)Math.Sqrt(sum);
            }
        }

        public static readonly SeparationMeasure HalfDeviation = new HalfDeviationMeasure();

        sealed class HalfDistanceMeasure : SeparationMeasure
        {
            public override float Measure(ScoreTable table)
            {
                return (GetMatchingDistance(table, GetMatchingMedian(table)) +
                    GetNonMatchingDistance(table, GetNonMatchingMedian(table))) / GetMedianDistance(table);
            }

            float GetMatchingDistance(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).Matching)
                        if (score < center)
                            sum += center - score;
                return (float)sum;
            }

            float GetNonMatchingDistance(ScoreTable table, float center)
            {
                double sum = 0;
                foreach (ScoreTable.Index index in table.GetAllIndexes())
                    foreach (float score in table.GetEntry(index).NonMatching)
                        if (score > center)
                            sum += score - center;
                return (float)sum;
            }
        }

        public static readonly SeparationMeasure HalfDistance = new HalfDistanceMeasure();

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
                foreach (float score in scores)
                    sum += score;
            }
            return (float)(sum / count);
        }

        protected float GetNonMatchingAverage(ScoreTable table)
        {
            double sum = 0;
            int count = 0;
            foreach (ScoreTable.Index index in table.GetAllIndexes())
            {
                float[] scores = table.GetEntry(index).Matching;
                count += scores.Length;
                foreach (float score in scores)
                    sum += score;
            }
            return (float)(sum / count);
        }
    }
}
