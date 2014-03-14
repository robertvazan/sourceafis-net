using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class MinutiaPairing : ICloneable
    {
        PairInfo[] CandidateIndex;
        PairInfo[] ProbeIndex;
        PairInfo[] PairList;
        int PairCount;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public int Count { get { return PairCount; } }
        public PairInfo LastAdded { get { return PairList[PairCount - 1]; } }

        public void SelectProbe(FingerprintTemplate probe)
        {
            ProbeIndex = new PairInfo[probe.Minutiae.Count];
            PairList = new PairInfo[probe.Minutiae.Count];
            for (int i = 0; i < PairList.Length; ++i)
                PairList[i] = new PairInfo();
            PairCount = 0;
        }

        public void SelectCandidate(FingerprintTemplate candidate)
        {
            if (CandidateIndex == null || CandidateIndex.Length < candidate.Minutiae.Count)
                CandidateIndex = new PairInfo[candidate.Minutiae.Count];
            else
            {
                for (int i = 0; i < CandidateIndex.Length; ++i)
                    CandidateIndex[i] = null;
            }
        }

        public void Reset(MinutiaPair root)
        {
            for (int i = 0; i < PairCount; ++i)
            {
                CandidateIndex[PairList[i].Pair.Candidate] = null;
                ProbeIndex[PairList[i].Pair.Probe] = null;
                PairList[i].SupportingEdges = 0;
            }
            CandidateIndex[root.Candidate] = ProbeIndex[root.Probe] = PairList[0];
            PairList[0].Pair = root;
            PairCount = 1;
        }

        public void Add(EdgePair edge)
        {
            CandidateIndex[edge.Neighbor.Candidate] = ProbeIndex[edge.Neighbor.Probe] = PairList[PairCount];
            PairList[PairCount].Pair = edge.Neighbor;
            PairList[PairCount].Reference = edge.Reference;
            ++PairCount;
        }

        public PairInfo GetByCandidate(int candidate)
        {
            return CandidateIndex[candidate];
        }

        public PairInfo GetByProbe(int probe)
        {
            return ProbeIndex[probe];
        }

        public bool IsProbePaired(int probe)
        {
            return ProbeIndex[probe] != null;
        }

        public bool IsCandidatePaired(int candidate)
        {
            return CandidateIndex[candidate] != null;
        }

        public PairInfo GetPair(int index)
        {
            return PairList[index];
        }

        public void AddSupportByProbe(int probe)
        {
            ++ProbeIndex[probe].SupportingEdges;
        }

        public void Log() { Logger.Log(this); }

        public object Clone()
        {
            MinutiaPairing clone = new MinutiaPairing();
            clone.ProbeIndex = (PairInfo[])ProbeIndex.Clone();
            clone.CandidateIndex = (PairInfo[])CandidateIndex.Clone();
            clone.PairList = (PairInfo[])PairList.CloneItems();
            clone.PairCount = PairCount;
            return clone;
        }
    }
}
