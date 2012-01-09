using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Simple
{
    class BestMatchSkipper
    {
        float[][] Collected;

        public struct PersonsSkipScore
        {
            public int Person;
            public float Score;
        }

        public BestMatchSkipper(int persons, int skip)
        {
            Collected = new float[skip + 1][];
            for (int i = 0; i < Collected.Length; ++i)
            {
                Collected[i] = new float[persons];
                for (int j = 0; j < Collected[i].Length; ++j)
                    Collected[i][j] = -1;
            }
        }

        public void AddScore(int person, float score)
        {
            for (int nth = Collected.Length - 1; nth >= 0; --nth)
            {
                if (Collected[nth][person] < score)
                {
                    if (nth + 1 < Collected.Length)
                        Collected[nth + 1][person] = Collected[nth][person];
                    Collected[nth][person] = score;
                }
            }
        }

        public float GetSkipScore(int person)
        {
            float score = 0;
            for (int nth = Collected.Length - 1; nth >= 0; --nth)
                if (Collected[nth][person] > 0)
                {
                    score = Collected[nth][person];
                    break;
                }
            return score;
        }

        public PersonsSkipScore[] GetSortedScores()
        {
            PersonsSkipScore[] results = new PersonsSkipScore[Collected[0].Length];
            for (int person = 0; person < results.Length; ++person)
            {
                results[person].Person = person;
                results[person].Score = GetSkipScore(person);
            }
            Array.Sort(results, (left, right) => Calc.Compare(right.Score, left.Score));
            return results;
        }
    }
}
