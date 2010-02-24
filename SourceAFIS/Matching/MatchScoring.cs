using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Matching
{
    public sealed class MatchScoring
    {
        public float PairCountFactor = 1;
        public float PairFractionFactor = 10;

        public float Score;

        public void Compute(MatchAnalysis analysis)
        {
            Score = 0;
            Score += PairCountFactor * analysis.PairCount;
            Score += PairFractionFactor * analysis.PairFraction;
        }
    }
}
