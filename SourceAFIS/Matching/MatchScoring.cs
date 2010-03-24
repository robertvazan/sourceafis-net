using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Matching
{
    public sealed class MatchScoring
    {
        [Parameter(Upper = 10)]
        public float PairCountFactor = 0.7f;
        [Parameter(Upper = 100)]
        public float PairFractionFactor = 9.35f;

        public float Compute(MatchAnalysis analysis)
        {
            float score = 0;
            score += PairCountFactor * analysis.PairCount;
            score += PairFractionFactor * analysis.PairFraction;
            return score;
        }
    }
}
