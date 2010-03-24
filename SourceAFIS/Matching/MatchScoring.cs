using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Matching
{
    public sealed class MatchScoring
    {
        [Parameter(Upper = 10)]
        public float PairCountFactor = 0.67f;
        [Parameter(Upper = 100)]
        public float PairFractionFactor = 9.35f;
        [Parameter(Upper = 10)]
        public float SupportedCountFactor = 0.27f;
        [Parameter(Upper = 10, Precision = 3)]
        public float EdgeCountFactor = 0.027f;

        public float Compute(MatchAnalysis analysis)
        {
            float score = 0;
            score += PairCountFactor * analysis.PairCount;
            score += SupportedCountFactor * analysis.SupportedCount;
            score += PairFractionFactor * analysis.PairFraction;
            score += EdgeCountFactor * analysis.EdgeCount;
            return score;
        }
    }
}
