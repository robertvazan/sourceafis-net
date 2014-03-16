using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public static class MatchScoring
    {
        const double PairCountFactor = 0.032;
        const double PairFractionFactor = 8.98;
        const double CorrectTypeFactor = 0.629;
        const double SupportedCountFactor = 0.193;
        const double EdgeCountFactor = 0.265;
        const double DistanceAccuracyFactor = 9.9;
        const double AngleAccuracyFactor = 2.79;

        public static double Compute(MatchAnalysis analysis)
        {
            double score = 0;
            
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
