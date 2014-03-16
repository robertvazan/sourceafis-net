using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class MinutiaPairing
    {
        PairInfo[] PairsByCandidate;
        PairInfo[] PairsByProbe;
        PairInfo[] PairList;
        int PairCount;

        public int Count { get { return PairCount; } }
        public PairInfo LastPair { get { return PairList[PairCount - 1]; } }

        public MinutiaPairing(FingerprintTemplate probe, FingerprintTemplate candidate, MinutiaPair root, MinutiaPairing recycled)
        {
            PairsByProbe = recycled != null ? recycled.PairsByProbe : new PairInfo[probe.Minutiae.Count];
            PairsByCandidate = recycled != null && recycled.PairsByCandidate.Length >= candidate.Minutiae.Count
                ? recycled.PairsByCandidate : new PairInfo[candidate.Minutiae.Count];
            if (recycled != null && recycled.PairsByCandidate.Length >= candidate.Minutiae.Count)
            {
                PairsByProbe = recycled.PairsByProbe;
                PairsByCandidate = recycled.PairsByCandidate;
                PairList = recycled.PairList;
                for (int i = 0; i < recycled.PairCount; ++i)
                {
                    PairsByProbe[PairList[i].Pair.Probe] = null;
                    PairsByCandidate[PairList[i].Pair.Candidate] = null;
                }
            }
            else
            {
                PairsByProbe = new PairInfo[probe.Minutiae.Count];
                PairsByCandidate = new PairInfo[candidate.Minutiae.Count];
                PairList = new PairInfo[probe.Minutiae.Count];
                for (int i = 0; i < PairList.Length; ++i)
                    PairList[i] = new PairInfo();
            }
            PairsByCandidate[root.Candidate] = PairsByProbe[root.Probe] = PairList[0];
            PairList[0].Pair = root;
            PairCount = 1;
        }

        public void AddPair(EdgePair edge)
        {
            PairsByCandidate[edge.Neighbor.Candidate] = PairsByProbe[edge.Neighbor.Probe] = PairList[PairCount];
            PairList[PairCount].Pair = edge.Neighbor;
            PairList[PairCount].Reference = edge.Reference;
            ++PairCount;
        }

        public PairInfo GetByCandidate(int candidate)
        {
            return PairsByCandidate[candidate];
        }

        public PairInfo GetByProbe(int probe)
        {
            return PairsByProbe[probe];
        }

        public bool IsProbePaired(int probe)
        {
            return PairsByProbe[probe] != null;
        }

        public bool IsCandidatePaired(int candidate)
        {
            return PairsByCandidate[candidate] != null;
        }

        public PairInfo GetPair(int index)
        {
            return PairList[index];
        }

        public void AddSupportByProbe(int probe)
        {
            ++PairsByProbe[probe].SupportingEdges;
        }
    }
}
