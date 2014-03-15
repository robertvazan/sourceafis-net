using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public static class MatchScoring
    {
        const float PairCountFactor = 0.032f;
        const float PairFractionFactor = 8.98f;
        const float CorrectTypeFactor = 0.629f;
        const float SupportedCountFactor = 0.193f;
        const float EdgeCountFactor = 0.265f;
        const float DistanceAccuracyFactor = 9.9f;
        const float AngleAccuracyFactor = 2.79f;

        public static float Compute(MatchAnalysis analysis)
        {
            float score = 0;
            
            score += PairCountFactor * analysis.PairCount;
            score += CorrectTypeFactor * analysis.CorrectTypeCount;
            score += SupportedCountFactor * analysis.SupportedCount;
            score += PairFractionFactor * analysis.PairFraction;
            score += EdgeCountFactor * analysis.EdgeCount;
            if (analysis.PairCount >= 2)
            {
                var maxDistanceError = EdgeLookup.MaxDistanceError * (analysis.PairCount - 1);
                score += DistanceAccuracyFactor * (maxDistanceError - analysis.DistanceErrorSum) / maxDistanceError;
                var maxAngleError = EdgeLookup.MaxAngleError * (analysis.PairCount - 1) * 2;
                score += AngleAccuracyFactor * (maxAngleError - analysis.AngleErrorSum) / maxAngleError;
            }
            
            return score;
        }
    }
}
