using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        Template Probe;
        Template Candidate;

        public int PairCount;
        public float PairFraction;

        public void SetProbe(Template probe)
        {
            Probe = probe;
        }

        public void SetCandidate(Template candidate)
        {
            Candidate = candidate;
        }

        public void Analyze(MinutiaPairing pairing)
        {
            PairCount = 0;
            foreach (MinutiaPair pair in pairing.GetPairs())
                ++PairCount;

            float probeFraction = PairCount / (float)Probe.Minutiae.Length;
            float candidateFraction = PairCount / (float)Candidate.Minutiae.Length;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
