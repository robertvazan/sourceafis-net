using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class ScoreTable : DatabaseLayout
    {
        public struct Entry
        {
            public float[] Matching;
            public float[] NonMatching;
        }

        public override int FingerCount { get { return Table.Length; } }
        public override int ViewCount { get { return Table[0].Length; } }

        public Entry[][] Table;

        [XmlIgnore]
        IEnumerable<Entry> Entries { get { return from finger in Table from entry in finger select entry; } }

        [XmlIgnore]
        public IEnumerable<float> Matching { get { return from entry in Entries from score in entry.Matching select score; } }

        [XmlIgnore]
        public IEnumerable<float> NonMatching { get { return from entry in Entries from score in entry.NonMatching select score; } }

        [XmlIgnore]
        public float this[TestPair pair]
        {
            get
            {
                Entry entry = Table[pair.Probe.Finger][pair.Probe.View];
                if (pair.IsMatching)
                    return entry.Matching[(ViewCount + pair.Candidate.View - pair.Probe.View - 1) % ViewCount];
                else
                    return entry.NonMatching[(FingerCount + pair.Candidate.Finger - pair.Probe.Finger - 1) % FingerCount];
            }
            set
            {
                Entry entry = Table[pair.Probe.Finger][pair.Probe.View];
                if (pair.IsMatching)
                    entry.Matching[(ViewCount + pair.Candidate.View - pair.Probe.View - 1) % ViewCount] = value;
                else
                    entry.NonMatching[(FingerCount + pair.Candidate.Finger - pair.Probe.Finger - 1) % FingerCount] = value;
            }
        }

        public void Initialize(int fingers, int views)
        {
            Table = (from finger in Enumerable.Range(0, fingers)
                     select (from view in Enumerable.Range(0, views)
                             select new Entry
                             {
                                 Matching = new float[views - 1],
                                 NonMatching = new float[fingers - 1]
                             }).ToArray()).ToArray();
        }

        public ScoreTable GetMultiFingerTable(MultiFingerPolicy policy)
        {
            ScoreTable combined = new ScoreTable();
            combined.Initialize(FingerCount, ViewCount);
            foreach (TestPair pair in AllPairs)
            {
                var probeSequence = GetConsequentFingers(pair.Probe).Take(policy.ExpectedCount).ToList();
                var candidateSequence = GetConsequentFingers(pair.Candidate).Take(policy.ExpectedCount).ToList();
                var scores = from offset in Enumerable.Range(0, probeSequence.Count)
                             let combinedPair = new TestPair(probeSequence[offset], candidateSequence[offset])
                             select this[combinedPair];
                combined[pair] = policy.Combine(scores.ToArray());
            }
            return combined;
        }
    }
}
