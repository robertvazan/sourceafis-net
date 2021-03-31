// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    class Score
    {
        int MinutiaCount;
        double MinutiaScore;
        double MinutiaFractionInProbe;
        double MinutiaFractionInCandidate;
        double MinutiaFraction;
        double MinutiaFractionScore;
        int SupportingEdgeSum;
        int EdgeCount;
        double EdgeScore;
        int SupportedMinutiaCount;
        double SupportedMinutiaScore;
        int MinutiaTypeHits;
        double MinutiaTypeScore;
        int DistanceErrorSum;
        int DistanceAccuracySum;
        double DistanceAccuracyScore;
        double AngleErrorSum;
        double AngleAccuracySum;
        double AngleAccuracyScore;
        double TotalScore;
        public double ShapedScore;

        public void Compute(MatcherThread thread)
        {
            MinutiaCount = thread.Count;
            MinutiaScore = Parameters.MinutiaScore * MinutiaCount;
            MinutiaFractionInProbe = thread.Count / (double)thread.Probe.Minutiae.Length;
            MinutiaFractionInCandidate = thread.Count / (double)thread.Candidate.Minutiae.Length;
            MinutiaFraction = 0.5 * (MinutiaFractionInProbe + MinutiaFractionInCandidate);
            MinutiaFractionScore = Parameters.MinutiaFractionScore * MinutiaFraction;
            SupportingEdgeSum = 0;
            SupportedMinutiaCount = 0;
            MinutiaTypeHits = 0;
            for (int i = 0; i < thread.Count; ++i)
            {
                var pair = thread.Tree[i];
                SupportingEdgeSum += pair.SupportingEdges;
                if (pair.SupportingEdges >= Parameters.MinSupportingEdges)
                    ++SupportedMinutiaCount;
                if (thread.Probe.Minutiae[pair.Probe].Type == thread.Candidate.Minutiae[pair.Candidate].Type)
                    ++MinutiaTypeHits;
            }
            EdgeCount = thread.Count + SupportingEdgeSum;
            EdgeScore = Parameters.EdgeScore * EdgeCount;
            SupportedMinutiaScore = Parameters.SupportedMinutiaScore * SupportedMinutiaCount;
            MinutiaTypeScore = Parameters.MinutiaTypeScore * MinutiaTypeHits;
            int innerDistanceRadius = Doubles.RoundToInt(Parameters.DistanceErrorFlatness * Parameters.MaxDistanceError);
            double innerAngleRadius = Parameters.AngleErrorFlatness * Parameters.MaxAngleError;
            DistanceErrorSum = 0;
            AngleErrorSum = 0;
            for (int i = 1; i < thread.Count; ++i)
            {
                var pair = thread.Tree[i];
                var probeEdge = new EdgeShape(thread.Probe.Minutiae[pair.ProbeRef], thread.Probe.Minutiae[pair.Probe]);
                var candidateEdge = new EdgeShape(thread.Candidate.Minutiae[pair.CandidateRef], thread.Candidate.Minutiae[pair.Candidate]);
                DistanceErrorSum += Math.Max(innerDistanceRadius, Math.Abs(probeEdge.Length - candidateEdge.Length));
                AngleErrorSum += Math.Max(innerAngleRadius, DoubleAngle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle));
                AngleErrorSum += Math.Max(innerAngleRadius, DoubleAngle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle));
            }
            DistanceAccuracyScore = 0;
            AngleAccuracyScore = 0;
            int distanceErrorPotential = Parameters.MaxDistanceError * Math.Max(0, thread.Count - 1);
            DistanceAccuracySum = distanceErrorPotential - DistanceErrorSum;
            DistanceAccuracyScore = Parameters.DistanceAccuracyScore * (distanceErrorPotential > 0 ? DistanceAccuracySum / (double)distanceErrorPotential : 0);
            double angleErrorPotential = Parameters.MaxAngleError * Math.Max(0, thread.Count - 1) * 2;
            AngleAccuracySum = angleErrorPotential - AngleErrorSum;
            AngleAccuracyScore = Parameters.AngleAccuracyScore * (angleErrorPotential > 0 ? AngleAccuracySum / angleErrorPotential : 0);
            TotalScore = MinutiaScore
                + MinutiaFractionScore
                + SupportedMinutiaScore
                + EdgeScore
                + MinutiaTypeScore
                + DistanceAccuracyScore
                + AngleAccuracyScore;
            ShapedScore = Shape(TotalScore);
        }
        static double Shape(double raw)
        {
            if (raw < Parameters.ThresholdFmrMax)
                return 0;
            if (raw < Parameters.ThresholdFmr2)
                return Interpolate(raw, Parameters.ThresholdFmrMax, Parameters.ThresholdFmr2, 0, 3);
            if (raw < Parameters.ThresholdFmr10)
                return Interpolate(raw, Parameters.ThresholdFmr2, Parameters.ThresholdFmr10, 3, 7);
            if (raw < Parameters.ThresholdFmr100)
                return Interpolate(raw, Parameters.ThresholdFmr10, Parameters.ThresholdFmr100, 10, 10);
            if (raw < Parameters.ThresholdFmr1000)
                return Interpolate(raw, Parameters.ThresholdFmr100, Parameters.ThresholdFmr1000, 20, 10);
            if (raw < Parameters.ThresholdFmr10K)
                return Interpolate(raw, Parameters.ThresholdFmr1000, Parameters.ThresholdFmr10K, 30, 10);
            if (raw < Parameters.ThresholdFmr100K)
                return Interpolate(raw, Parameters.ThresholdFmr10K, Parameters.ThresholdFmr100K, 40, 10);
            return (raw - Parameters.ThresholdFmr100K) / (Parameters.ThresholdFmr100K - Parameters.ThresholdFmr100) * 30 + 50;
        }
        static double Interpolate(double raw, double min, double max, double start, double length)
        {
            return (raw - min) / (max - min) * length + start;
        }
    }
}
