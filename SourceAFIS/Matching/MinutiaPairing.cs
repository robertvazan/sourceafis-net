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

        public void SelectProbe(FingerprintTemplate probe)
        {
            PairsByProbe = new PairInfo[probe.Minutiae.Count];
            PairList = new PairInfo[probe.Minutiae.Count];
            for (int i = 0; i < PairList.Length; ++i)
                PairList[i] = new PairInfo();
            PairCount = 0;
        }

        public void SelectCandidate(FingerprintTemplate candidate)
        {
            if (PairsByCandidate == null || PairsByCandidate.Length < candidate.Minutiae.Count)
                PairsByCandidate = new PairInfo[candidate.Minutiae.Count];
            else
            {
                for (int i = 0; i < PairsByCandidate.Length; ++i)
                    PairsByCandidate[i] = null;
            }
        }

        public void ResetPairing(MinutiaPair root)
        {
            for (int i = 0; i < PairCount; ++i)
            {
                PairsByCandidate[PairList[i].Pair.Candidate] = null;
                PairsByProbe[PairList[i].Pair.Probe] = null;
                PairList[i].SupportingEdges = 0;
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
