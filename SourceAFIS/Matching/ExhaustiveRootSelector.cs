using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class ExhaustiveRootSelector : IRootSelector
    {
        int ProbeCount;
        int CandidateCount;

        public void SetProbe(Template probe)
        {
            ProbeCount = probe.Minutiae.Length;
        }

        public void SetCandidate(Template candidate)
        {
            CandidateCount = candidate.Minutiae.Length;
        }

        public IEnumerable<MinutiaPair> GetRoots()
        {
            for (int probe = 0; probe < ProbeCount; ++probe)
                for (int candidate = 0; candidate < CandidateCount; ++candidate)
                    yield return new MinutiaPair(probe, candidate);
        }
    }
}
