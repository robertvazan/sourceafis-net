using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class MatchScoring
    {
        [Parameter(Upper = 10, Precision = 3)]
        public float PairCountFactor = 0.025f;
        [Parameter(Upper = 100)]
        public float PairFractionFactor = 7.34f;
        [Parameter(Upper = 10, Precision = 3)]
        public float CorrectTypeFactor = 0.629f;
        [Parameter(Upper = 10, Precision = 3)]
        public float SupportedCountFactor = 0.19f;
        [Parameter(Upper = 10, Precision = 3)]
        public float EdgeCountFactor = 0.267f;
        [Parameter(Upper = 100)]
        public float DistanceAccuracyFactor = 9.9f;
        [Parameter(Upper = 100)]
        public float AngleAccuracyFactor = 2.8f;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public float Compute(MatchAnalysis analysis)
        {
            float score = 0;
            
            score += PairCountFactor * analysis.PairCount;
            score += CorrectTypeFactor * analysis.CorrectTypeCount;
            score += SupportedCountFactor * analysis.SupportedCount;
            score += PairFractionFactor * analysis.PairFraction;
            score += EdgeCountFactor * analysis.EdgeCount;
            if (analysis.PairCount >= 2)
            {
                var maxDistanceError = analysis.MaxDistanceError * (analysis.PairCount - 1);
                score += DistanceAccuracyFactor * (maxDistanceError - analysis.DistanceErrorSum) / maxDistanceError;
                var maxAngleError = analysis.MaxAngleError * (analysis.PairCount - 1) * 2;
                score += AngleAccuracyFactor * (maxAngleError - analysis.AngleErrorSum) / maxAngleError;
            }
            
            if (Logger.IsActive)
                Logger.Log(score);

            return score;
        }
    }
}
