using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class MatchAnalysis
    {
        [Parameter(Lower = 0, Upper = 5)]
        public int MinSupportingEdges = 1;
        [Parameter]
        public float DistanceErrorFlatness = 0.62f;
        [Parameter]
        public float AngleErrorFlatness = 0.24f;

        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        public int MaxDistanceError;
        public byte MaxAngleError;
        
        public int PairCount;
        public int CorrectTypeCount;
        public int SupportedCount;
        public float PairFraction;
        public int EdgeCount;
        public int DistanceErrorSum;
        public int AngleErrorSum;

        public void Analyze(MinutiaPairing pairing, EdgeLookup lookup, Template probe, Template candidate)
        {
            MaxDistanceError = lookup.MaxDistanceError;
            MaxAngleError = lookup.MaxAngleError;
            var innerDistanceRadius = Convert.ToInt32(DistanceErrorFlatness * MaxDistanceError);
            var innerAngleRadius = Convert.ToInt32(AngleErrorFlatness * MaxAngleError);

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

            float probeFraction = PairCount / (float)probe.Minutiae.Length;
            float candidateFraction = PairCount / (float)candidate.Minutiae.Length;
            PairFraction = (probeFraction + candidateFraction) / 2;
        }
    }
}
