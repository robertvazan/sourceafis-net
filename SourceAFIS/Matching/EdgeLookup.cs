using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class EdgeLookup
    {
        public struct LookupResult
        {
            public MinutiaPair Pair;
            public int Distance;
        }

        public static void FindMatchingPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar, List<LookupResult> results)
        {
            byte complementaryAngleError = Angle.Complementary(FingerprintMatcher.MaxAngleError);
            results.Clear();
            Range range = new Range();

            for (int candidateIndex = 0; candidateIndex < candidateStar.Length; ++candidateIndex)
            {
                NeighborEdge candidateEdge = candidateStar[candidateIndex];

                while (range.Begin < probeStar.Length && probeStar[range.Begin].Edge.Length < candidateEdge.Edge.Length - FingerprintMatcher.MaxDistanceError)
                    ++range.Begin;
                if (range.End < range.Begin)
                    range.End = range.Begin;
                while (range.End < probeStar.Length && probeStar[range.End].Edge.Length <= candidateEdge.Edge.Length + FingerprintMatcher.MaxDistanceError)
                    ++range.End;

                for (int probeIndex = range.Begin; probeIndex < range.End; ++probeIndex)
                {
                    var probeEdge = probeStar[probeIndex];
                    byte referenceDiff = Angle.Difference(probeEdge.Edge.ReferenceAngle, candidateEdge.Edge.ReferenceAngle);
                    if (referenceDiff <= FingerprintMatcher.MaxAngleError || referenceDiff >= complementaryAngleError)
                    {
                        byte neighborDiff = Angle.Difference(probeEdge.Edge.NeighborAngle, candidateEdge.Edge.NeighborAngle);
                        if (neighborDiff <= FingerprintMatcher.MaxAngleError || neighborDiff >= complementaryAngleError)
                            results.Add(new LookupResult()
                            {
                                Pair = new MinutiaPair(probeEdge.Neighbor, candidateEdge.Neighbor),
                                Distance = candidateEdge.Edge.Length
                            });
                    }
                }
            }
        }

        public static bool MatchingEdges(EdgeShape probe, EdgeShape candidate)
        {
            int lengthDelta = probe.Length - candidate.Length;
            if (lengthDelta >= -FingerprintMatcher.MaxDistanceError && lengthDelta <= FingerprintMatcher.MaxDistanceError)
            {
                byte complementaryAngleError = Angle.Complementary(FingerprintMatcher.MaxAngleError);
                byte referenceDelta = Angle.Difference(probe.ReferenceAngle, candidate.ReferenceAngle);
                if (referenceDelta <= FingerprintMatcher.MaxAngleError || referenceDelta >= complementaryAngleError)
                {
                    byte neighborDelta = Angle.Difference(probe.NeighborAngle, candidate.NeighborAngle);
                    if (neighborDelta <= FingerprintMatcher.MaxAngleError || neighborDelta >= complementaryAngleError)
                        return true;
                }
            }
            return false;
        }

        public static int ComputeHash(EdgeShape edge)
        {
            return (edge.ReferenceAngle / FingerprintMatcher.MaxAngleError << 24) + (edge.NeighborAngle / FingerprintMatcher.MaxAngleError << 16) + edge.Length / FingerprintMatcher.MaxDistanceError;
        }

        public static IEnumerable<int> HashCoverage(EdgeShape edge)
        {
            int minLengthBin = (edge.Length - FingerprintMatcher.MaxDistanceError) / FingerprintMatcher.MaxDistanceError;
            int maxLengthBin = (edge.Length + FingerprintMatcher.MaxDistanceError) / FingerprintMatcher.MaxDistanceError;
            int angleBins = MathEx.DivRoundUp(256, FingerprintMatcher.MaxAngleError);
            int minReferenceBin = Angle.Difference(edge.ReferenceAngle, FingerprintMatcher.MaxAngleError) / FingerprintMatcher.MaxAngleError;
            int maxReferenceBin = Angle.Add(edge.ReferenceAngle, FingerprintMatcher.MaxAngleError) / FingerprintMatcher.MaxAngleError;
            int endReferenceBin = (maxReferenceBin + 1) % angleBins;
            int minNeighborBin = Angle.Difference(edge.NeighborAngle, FingerprintMatcher.MaxAngleError) / FingerprintMatcher.MaxAngleError;
            int maxNeighborBin = Angle.Add(edge.NeighborAngle, FingerprintMatcher.MaxAngleError) / FingerprintMatcher.MaxAngleError;
            int endNeighborBin = (maxNeighborBin + 1) % angleBins;
            for (int lengthBin = minLengthBin; lengthBin <= maxLengthBin; ++lengthBin)
                for (int referenceBin = minReferenceBin; referenceBin != endReferenceBin; referenceBin = (referenceBin + 1) % angleBins)
                    for (int neighborBin = minNeighborBin; neighborBin != endNeighborBin; neighborBin = (neighborBin + 1) % angleBins)
                        yield return (referenceBin << 24) + (neighborBin << 16) + lengthBin;
        }
    }
}
