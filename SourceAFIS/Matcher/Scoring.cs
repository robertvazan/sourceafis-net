// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Configuration;
using SourceAFIS.Features;
using SourceAFIS.Primitives;

namespace SourceAFIS.Matcher
{
    class Scoring
    {
        public static void Compute(FingerprintTemplate probe, FingerprintTemplate candidate, PairingGraph pairing, ScoringData score)
        {
            var pminutiae = probe.Minutiae;
            var cminutiae = candidate.Minutiae;
            score.MinutiaCount = pairing.Count;
            score.MinutiaScore = Parameters.MinutiaScore * score.MinutiaCount;
            score.MinutiaFractionInProbe = pairing.Count / (double)pminutiae.Length;
            score.MinutiaFractionInCandidate = pairing.Count / (double)cminutiae.Length;
            score.MinutiaFraction = 0.5 * (score.MinutiaFractionInProbe + score.MinutiaFractionInCandidate);
            score.MinutiaFractionScore = Parameters.MinutiaFractionScore * score.MinutiaFraction;
            score.SupportingEdgeSum = 0;
            score.SupportedMinutiaCount = 0;
            score.MinutiaTypeHits = 0;
            for (int i = 0; i < pairing.Count; ++i)
            {
                var pair = pairing.Tree[i];
                score.SupportingEdgeSum += pair.SupportingEdges;
                if (pair.SupportingEdges >= Parameters.MinSupportingEdges)
                    ++score.SupportedMinutiaCount;
                if (pminutiae[pair.Probe].Type == cminutiae[pair.Candidate].Type)
                    ++score.MinutiaTypeHits;
            }
            score.EdgeCount = pairing.Count + score.SupportingEdgeSum;
            score.EdgeScore = Parameters.EdgeScore * score.EdgeCount;
            score.SupportedMinutiaScore = Parameters.SupportedMinutiaScore * score.SupportedMinutiaCount;
            score.MinutiaTypeScore = Parameters.MinutiaTypeScore * score.MinutiaTypeHits;
            int innerDistanceRadius = Doubles.RoundToInt(Parameters.DistanceErrorFlatness * Parameters.MaxDistanceError);
            double innerAngleRadius = Parameters.AngleErrorFlatness * Parameters.MaxAngleError;
            score.DistanceErrorSum = 0;
            score.AngleErrorSum = 0;
            for (int i = 1; i < pairing.Count; ++i)
            {
                var pair = pairing.Tree[i];
                var probeEdge = new EdgeShape(pminutiae[pair.ProbeRef], pminutiae[pair.Probe]);
                var candidateEdge = new EdgeShape(cminutiae[pair.CandidateRef], cminutiae[pair.Candidate]);
                score.DistanceErrorSum += Math.Max(innerDistanceRadius, Math.Abs(probeEdge.Length - candidateEdge.Length));
                score.AngleErrorSum += Math.Max(innerAngleRadius, DoubleAngle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle));
                score.AngleErrorSum += Math.Max(innerAngleRadius, DoubleAngle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle));
            }
            score.DistanceAccuracyScore = 0;
            score.AngleAccuracyScore = 0;
            int distanceErrorPotential = Parameters.MaxDistanceError * Math.Max(0, pairing.Count - 1);
            score.DistanceAccuracySum = distanceErrorPotential - score.DistanceErrorSum;
            score.DistanceAccuracyScore = Parameters.DistanceAccuracyScore * (distanceErrorPotential > 0 ? score.DistanceAccuracySum / (double)distanceErrorPotential : 0);
            double angleErrorPotential = Parameters.MaxAngleError * Math.Max(0, pairing.Count - 1) * 2;
            score.AngleAccuracySum = angleErrorPotential - score.AngleErrorSum;
            score.AngleAccuracyScore = Parameters.AngleAccuracyScore * (angleErrorPotential > 0 ? score.AngleAccuracySum / angleErrorPotential : 0);
            score.TotalScore = score.MinutiaScore
                + score.MinutiaFractionScore
                + score.SupportedMinutiaScore
                + score.EdgeScore
                + score.MinutiaTypeScore
                + score.DistanceAccuracyScore
                + score.AngleAccuracyScore;
            score.ShapedScore = Shape(score.TotalScore);
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
