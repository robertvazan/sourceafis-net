using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        public int PairCount;
        public float PairFraction;

        public void Analyze(MinutiaPairing pairing, Template probe, Template candidate)
        {
            PairCount = 0;
            foreach (MinutiaPair pair in pairing.GetPairs())
                ++PairCount;

            float probeFraction = PairCount / (float)probe.Minutiae.Length;
            float candidateFraction = PairCount / (float)candidate.Minutiae.Length;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
