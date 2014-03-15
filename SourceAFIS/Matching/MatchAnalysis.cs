using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        const int MinSupportingEdges = 1;
        const float DistanceErrorFlatness = 0.69f;
        const float AngleErrorFlatness = 0.27f;

        public int PairCount;
        public int CorrectTypeCount;
        public int SupportedCount;
        public float PairFraction;
        public int EdgeCount;
        public int DistanceErrorSum;
        public int AngleErrorSum;

        public void Analyze(MinutiaPairing pairing, FingerprintTemplate probe, FingerprintTemplate candidate)
        {
            var innerDistanceRadius = Convert.ToInt32(DistanceErrorFlatness * EdgeLookup.MaxDistanceError);
            var innerAngleRadius = Convert.ToInt32(AngleErrorFlatness * EdgeLookup.MaxAngleError);

            PairCount = pairing.Count;

            EdgeCount = 0;
            SupportedCount = 0;
            CorrectTypeCount = 0;
            DistanceErrorSum = 0;
            AngleErrorSum = 0;

            for (int i = 0; i < PairCount; ++i)
            {
                PairInfo pair = pairing.GetPair(i);
                if (pair.SupportingEdges >= MinSupportingEdges)
                    ++SupportedCount;
                EdgeCount += pair.SupportingEdges + 1;
                if (probe.Minutiae[pair.Pair.Probe].Type == candidate.Minutiae[pair.Pair.Candidate].Type)
                    ++CorrectTypeCount;
                if (i > 0)
                {
                    var probeEdge = EdgeConstructor.Construct(probe, pair.Reference.Probe, pair.Pair.Probe);
                    var candidateEdge = EdgeConstructor.Construct(candidate, pair.Reference.Candidate, pair.Pair.Candidate);
                    DistanceErrorSum += Math.Abs(probeEdge.Length - candidateEdge.Length);
                    AngleErrorSum += Math.Max(innerDistanceRadius, Angle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle));
                    AngleErrorSum += Math.Max(innerAngleRadius, Angle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle));
                }
            }

            float probeFraction = PairCount / (float)probe.Minutiae.Count;
            float candidateFraction = PairCount / (float)candidate.Minutiae.Count;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
