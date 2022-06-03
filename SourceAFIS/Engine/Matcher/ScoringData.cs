// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Matcher
{
    class ScoringData
    {
        public int MinutiaCount;
        public double MinutiaScore;
        public double MinutiaFractionInProbe;
        public double MinutiaFractionInCandidate;
        public double MinutiaFraction;
        public double MinutiaFractionScore;
        public int SupportingEdgeSum;
        public int EdgeCount;
        public double EdgeScore;
        public int SupportedMinutiaCount;
        public double SupportedMinutiaScore;
        public int MinutiaTypeHits;
        public double MinutiaTypeScore;
        public int DistanceErrorSum;
        public int DistanceAccuracySum;
        public double DistanceAccuracyScore;
        public double AngleErrorSum;
        public double AngleAccuracySum;
        public double AngleAccuracyScore;
        public double TotalScore;
        public double ShapedScore;
    }
}
