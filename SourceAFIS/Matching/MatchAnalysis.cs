using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        [Parameter(Lower = 0, Upper = 5)]
        public int MinSupportingEdges = 2;

        public int PairCount;
        public int CorrectTypeCount;
        public int SupportedCount;
        public float PairFraction;
        public int EdgeCount;

        public void Analyze(MinutiaPairing pairing, Template probe, Template candidate)
        {
            PairCount = pairing.Count;

            EdgeCount = 0;
            SupportedCount = 0;
            CorrectTypeCount = 0;

            for (int i = 0; i < PairCount; ++i)
            {
                MinutiaPair pair = pairing.GetPair(i);
                int support = pairing.GetSupportByProbe(pair.Probe);
                if (support >= MinSupportingEdges)
                    ++SupportedCount;
                EdgeCount += support + 1;
                if (probe.Minutiae[pair.Probe].Type == candidate.Minutiae[pair.Candidate].Type)
                    ++CorrectTypeCount;
            }

            float probeFraction = PairCount / (float)probe.Minutiae.Length;
            float candidateFraction = PairCount / (float)candidate.Minutiae.Length;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
