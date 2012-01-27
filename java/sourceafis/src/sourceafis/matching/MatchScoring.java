package sourceafis.matching;

import sourceafis.general.DetailLogger;
import sourceafis.meta.Parameter;

public class MatchScoring {
	@Parameter(upper=10 , precision=3)
	public float PairCountFactor = 0.032f;
	@Parameter(upper = 100)
	public float PairFractionFactor = 8.98f;
	@Parameter(upper =10 , precision=3)
	public float CorrectTypeFactor = 0.629f;
	@Parameter(precision = 3)
	public float SupportedCountFactor = 0.193f;
	@Parameter(precision = 3)
	public float EdgeCountFactor = 0.265f;
	@Parameter(upper=100)
	public float DistanceAccuracyFactor = 9.9f;
    @Parameter(upper = 100)
    public float AngleAccuracyFactor = 2.79f;
	public DetailLogger.Hook logger = DetailLogger.off;

	public float Compute(MatchAnalysis analysis) {
		float score = 0;

		score += PairCountFactor * analysis.pairCount;
		score += CorrectTypeFactor * analysis.correctTypeCount;
		score += SupportedCountFactor * analysis.supportedCount;
		score += PairFractionFactor * analysis.pairFraction;
		score += EdgeCountFactor * analysis.edgeCount;
		if (analysis.pairCount >= 2)
        {
            int maxDistanceError = analysis.maxDistanceError * (analysis.pairCount - 1);
            score += DistanceAccuracyFactor * (maxDistanceError - analysis.distanceErrorSum) / maxDistanceError;
            int maxAngleError = analysis.maxAngleError * (analysis.pairCount - 1) * 2;
            score += AngleAccuracyFactor * (maxAngleError - analysis.angleErrorSum) / maxAngleError;
        }
		
		
		if (logger.isActive())
			logger.log(score);

		return score;
	}
}
