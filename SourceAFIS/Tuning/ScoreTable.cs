using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning
{
    public sealed class ScoreTable
    {
        public struct Entry
        {
            public float[] Matching;
            public float[] NonMatching;
        }

        public Entry[][] Table;

        public void Initialize(TestDatabase.Database database)
        {
            Table = new Entry[database.Fingers.Count][];
            for (int finger = 0; finger < database.Fingers.Count; ++finger)
                Table[finger] = new Entry[database.Fingers[finger].Views.Count];
        }

        public ScoreTable GetMultiFingerTable(MultiFingerPolicy policy)
        {
            ScoreTable combined = new ScoreTable();
            combined.Table = new Entry[Table.Length][];
            for (int finger = 0; finger < Table.Length; ++finger)
            {
                combined.Table[finger] = new Entry[Table[finger].Length];
                for (int view = 0; view < Table[finger].Length; ++view)
                {
                    combined.Table[finger][view].Matching = new float[Table[finger][view].Matching.Length];
                    for (int pair = 0; pair < Table[finger][view].Matching.Length; ++pair)
                    {
                        List<float> scores = new List<float>();
                        for (int multi = 0; multi < Math.Min(policy.ExpectedCount, Table.Length); ++multi)
                        {
                            if (view < Table[(finger + multi) % Table.Length].Length
                                && pair < Table[(finger + multi) % Table.Length][view].Matching.Length)
                            {
                                scores.Add(Table[(finger + multi) % Table.Length][view].Matching[pair]);
                            }
                        }
                        combined.Table[finger][view].Matching[pair] = policy.Combine(scores.ToArray());
                    }

                    combined.Table[finger][view].NonMatching = new float[Table[finger][view].NonMatching.Length];
                    for (int pair = 0; pair < Table[finger][view].NonMatching.Length; ++pair)
                    {
                        List<float> scores = new List<float>();
                        for (int multi = 0; multi < Math.Min(policy.ExpectedCount, Table[finger].Length); ++multi)
                        {
                            if (pair < Table[finger][(view + multi) % Table[finger].Length].NonMatching.Length)
                            {
                                scores.Add(Table[finger][(view + multi) % Table[finger].Length].NonMatching[pair]);
                            }
                        }
                        combined.Table[finger][view].NonMatching[pair] = policy.Combine(scores.ToArray());
                    }
                }
            }
            return combined;
        }
    }
}
