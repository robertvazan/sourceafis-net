package sourceafis.matching;

import sourceafis.general.DetailLogger;
import sourceafis.meta.Parameter;

public class MatchScoring {
	@Parameter
	public float PairCountFactor = 0.48f;
	@Parameter
	public float PairFractionFactor = 6.7f;
	@Parameter
	public float CorrectTypeFactor = 0.1f;
	@Parameter
	public float SupportedCountFactor = 0.4f;
	@Parameter(precision = 3)
	public float EdgeCountFactor = 0.208f;

	public DetailLogger.Hook logger = DetailLogger.off;

	public float Compute(MatchAnalysis analysis) {
		float score = 0;

		score += PairCountFactor * analysis.PairCount;
		score += CorrectTypeFactor * analysis.CorrectTypeCount;
		score += SupportedCountFactor * analysis.SupportedCount;
		score += PairFractionFactor * analysis.PairFraction;
		score += EdgeCountFactor * analysis.EdgeCount;
		
		if (logger.isActive())
			logger.log(score);

		return score;
	}
}
