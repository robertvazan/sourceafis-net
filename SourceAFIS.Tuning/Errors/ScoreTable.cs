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
        public override int MatchingPerProbe { get { return Table[0][0].Matching.Length; } }
        public override int NonMatchingPerProbe { get { return Table[0][0].NonMatching.Length; } }

        public Entry[][] Table;

        [XmlIgnore]
        IEnumerable<Entry> Entries { get { return from finger in Table from entry in finger select entry; } }

        [XmlIgnore]
        public IEnumerable<float> Matching { get { return from entry in Entries from score in entry.Matching select score; } }

        [XmlIgnore]
        public IEnumerable<float> NonMatching { get { return from entry in Entries from score in entry.NonMatching select score; } }

        int GetCyclicOffset(int start, int at, int cycle) { return (at - start + cycle) % cycle; }
        int GetViewOffset(int start, int at) { return GetCyclicOffset(start, at, ViewCount); }
        int GetFingerOffset(int start, int at) { return GetCyclicOffset(start, at, FingerCount); }
        int GetDistinctViewOffset(int start, int at) { return GetViewOffset(start + 1, at); }
        int GetDistinctFingerOffset(int start, int at) { return GetFingerOffset(start + 1, at); }
        int GetMatchingOffset(TestPair pair) { return GetDistinctViewOffset(pair.Probe.View, pair.Candidate.View); }
        int GetNonMatchingOffset(TestPair pair)
        {
            return GetViewOffset(pair.Probe.View, pair.Candidate.View) * (FingerCount - 1)
                + GetDistinctFingerOffset(pair.Probe.Finger, pair.Candidate.Finger);
        }

        [XmlIgnore]
        public float this[TestPair pair]
        {
            get
            {
                Entry entry = Table[pair.Probe.Finger][pair.Probe.View];
                if (pair.IsMatching)
                    return entry.Matching[GetMatchingOffset(pair)];
                else
                    return entry.NonMatching[GetNonMatchingOffset(pair)];
            }
            set
            {
                Entry entry = Table[pair.Probe.Finger][pair.Probe.View];
                if (pair.IsMatching)
                    entry.Matching[GetMatchingOffset(pair)] = value;
                else
                    entry.NonMatching[GetNonMatchingOffset(pair)] = value;
            }
        }

        public void Initialize(DatabaseLayout layout)
        {
            Table = (from finger in Enumerable.Range(0, layout.FingerCount)
                     select (from view in Enumerable.Range(0, layout.ViewCount)
                             select new Entry
                             {
                                 Matching = new float[layout.MatchingPerProbe],
                                 NonMatching = new float[layout.NonMatchingPerProbe]
                             }).ToArray()).ToArray();
        }

        public ScoreTable GetMultiFingerTable(MultiFingerPolicy policy)
        {
            ScoreTable combined = new ScoreTable();
            combined.Initialize(this);
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
