using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Matching
{
    public sealed class MatchScoring
    {
        public float PairCountFactor = 1;
        public float PairFractionFactor = 10;

        public float Compute(MatchAnalysis analysis)
        {
            float score = 0;
            score += PairCountFactor * analysis.PairCount;
            score += PairFractionFactor * analysis.PairFraction;
            return score;
        }
    }
}
