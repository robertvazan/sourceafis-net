using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Dummy;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class MinutiaPairing : ICloneable
    {
        int[] ProbeByCandidate;
        int[] CandidateByProbe;
        int[] SupportingEdgesByProbe;
        MinutiaPair[] PairList;
        int PairCount;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public int Count { get { return PairCount; } }
        public MinutiaPair LastAdded { get { return PairList[PairCount - 1]; } }

        public void SelectProbe(Template probe)
        {
            CandidateByProbe = new int[probe.Minutiae.Length];
            for (int i = 0; i < CandidateByProbe.Length; ++i)
                CandidateByProbe[i] = -1;
            SupportingEdgesByProbe = new int[probe.Minutiae.Length];
            PairList = new MinutiaPair[probe.Minutiae.Length];
            PairCount = 0;
        }

        public void SelectCandidate(Template candidate)
        {
            if (ProbeByCandidate == null || ProbeByCandidate.Length < candidate.Minutiae.Length)
                ProbeByCandidate = new int[candidate.Minutiae.Length];
            for (int i = 0; i < ProbeByCandidate.Length; ++i)
                ProbeByCandidate[i] = -1;
        }

        public void Reset()
        {
            for (int i = 0; i < PairCount; ++i)
            {
                ProbeByCandidate[PairList[i].Candidate] = -1;
                CandidateByProbe[PairList[i].Probe] = -1;
                SupportingEdgesByProbe[PairList[i].Probe] = 0;
            }
            PairCount = 0;
        }

        public void Add(MinutiaPair pair)
        {
            ProbeByCandidate[pair.Candidate] = pair.Probe;
            CandidateByProbe[pair.Probe] = pair.Candidate;
            PairList[PairCount] = pair;
            ++PairCount;
        }

        public int GetProbeByCandidate(int candidate)
        {
            return ProbeByCandidate[candidate];
        }

        public int GetCandidateByProbe(int probe)
        {
            return CandidateByProbe[probe];
        }

        public bool IsProbePaired(int probe)
        {
            return CandidateByProbe[probe] >= 0;
        }

        public bool IsCandidatePaired(int candidate)
        {
            return ProbeByCandidate[candidate] >= 0;
        }

        public MinutiaPair GetPair(int index)
        {
            return PairList[index];
        }

        public void AddSupportByProbe(int probe)
        {
            ++SupportingEdgesByProbe[probe];
        }

        public int GetSupportByProbe(int probe)
        {
            return SupportingEdgesByProbe[probe];
        }

        public void Log() { Logger.Log(this); }

        public object Clone()
        {
            MinutiaPairing clone = new MinutiaPairing();
            clone.CandidateByProbe = (int[])CandidateByProbe.Clone();
            clone.ProbeByCandidate = (int[])ProbeByCandidate.Clone();
            clone.SupportingEdgesByProbe = (int[])SupportingEdgesByProbe.Clone();
            clone.PairList = (MinutiaPair[])PairList.Clone();
            clone.PairCount = PairCount;
            return clone;
        }
    }
}
