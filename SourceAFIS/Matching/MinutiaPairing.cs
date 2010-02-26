using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class MinutiaPairing : ICloneable
    {
        int[] ProbeByCandidate;
        int[] CandidateByProbe;
        MinutiaPair LastPair;

        public MinutiaPair LastAdded { get { return LastPair; } }

        public void SelectProbe(Template probe)
        {
            CandidateByProbe = new int[probe.Minutiae.Length];
        }

        public void SelectCandidate(Template candidate)
        {
            if (ProbeByCandidate == null || ProbeByCandidate.Length < candidate.Minutiae.Length)
                ProbeByCandidate = new int[candidate.Minutiae.Length];
        }

        public void Reset()
        {
            for (int i = 0; i < ProbeByCandidate.Length; ++i)
                ProbeByCandidate[i] = -1;
            for (int i = 0; i < CandidateByProbe.Length; ++i)
                CandidateByProbe[i] = -1;
        }

        public void Add(MinutiaPair pair)
        {
            ProbeByCandidate[pair.Candidate] = pair.Probe;
            CandidateByProbe[pair.Probe] = pair.Candidate;
            LastPair = pair;
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

        public IEnumerable<MinutiaPair> GetPairs()
        {
            for (int probe = 0; probe < CandidateByProbe.Length; ++probe)
                if (CandidateByProbe[probe] >= 0)
                    yield return new MinutiaPair(probe, CandidateByProbe[probe]);
        }

        public object Clone()
        {
            MinutiaPairing clone = new MinutiaPairing();
            clone.CandidateByProbe = (int[])CandidateByProbe.Clone();
            clone.ProbeByCandidate = (int[])ProbeByCandidate.Clone();
            clone.LastPair = LastPair;
            return clone;
        }
    }
}
