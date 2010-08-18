using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class ScoreTable
    {
        public struct Entry
        {
            public float[] Matching;
            public float[] NonMatching;
        }

        public struct Index
        {
            [XmlAttribute]
            public int Finger;
            [XmlAttribute]
            public int View;

            public Index(int finger, int view)
            {
                Finger = finger;
                View = view;
            }
        }

        public Entry[][] Table;

        [XmlIgnore]
        public IEnumerable<Entry> Entries { get { return from row in Table from entry in row select entry; } }

        [XmlIgnore]
        public IEnumerable<float> Matching { get { return from entry in Entries from score in entry.Matching select score; } }

        [XmlIgnore]
        public IEnumerable<float> NonMatching { get { return from entry in Entries from score in entry.NonMatching select score; } }

        public void Initialize(TestDatabase database)
        {
            Table = new Entry[database.Fingers.Count][];
            for (int finger = 0; finger < database.Fingers.Count; ++finger)
                Table[finger] = new Entry[database.ViewCount];
        }

        public ScoreTable GetMultiFingerTable(MultiFingerPolicy policy)
        {
            return new ScoreTable
            {
                Table = (from finger in Enumerable.Range(0, Table.Length)
                         select (from view in Enumerable.Range(0, Table[finger].Length)
                                 let combinedFingers = (from multi in Enumerable.Range(0, Math.Min(policy.ExpectedCount, Table.Length))
                                                        let finger2 = (finger + multi) % Table.Length
                                                        select finger2).Take(policy.ExpectedCount)
                                 let newMatching = (from pair in Enumerable.Range(0, Table[finger][view].Matching.Length)
                                                    select policy.Combine((from finger2 in combinedFingers
                                                                           let matching2 = Table[finger2][view].Matching
                                                                           select matching2[pair]).ToArray())).ToArray()
                                 let newNonMatching = (from pair in Enumerable.Range(0, Table[finger][view].NonMatching.Length)
                                                       select policy.Combine((from finger2 in combinedFingers
                                                                              let nonmatching2 = Table[finger2][view].NonMatching
                                                                              select nonmatching2[pair]).ToArray())).ToArray()
                                 select new Entry { Matching = newMatching, NonMatching = newNonMatching }).ToArray()).ToArray()
            };
        }
    }
}
