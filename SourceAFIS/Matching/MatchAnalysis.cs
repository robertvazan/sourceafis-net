using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        [Parameter(Lower = 0, Upper = 5)]
        public int MinSupportingEdges = 1;

        public int PairCount;
        public int SupportedCount;
        public float PairFraction;
        public int EdgeCount;

        public void Analyze(MinutiaPairing pairing, Template probe, Template candidate)
        {
            PairCount = pairing.Count;

            EdgeCount = 0;
            SupportedCount = 0;
            for (int i = 0; i < PairCount; ++i)
            {
                int support = pairing.GetSupportByProbe(pairing.GetPair(i).Probe);
                if (support >= MinSupportingEdges)
                    ++SupportedCount;
                EdgeCount += support + 1;
            }

            float probeFraction = SupportedCount / (float)probe.Minutiae.Length;
            float candidateFraction = SupportedCount / (float)candidate.Minutiae.Length;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
