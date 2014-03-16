using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class EdgeLookup
    {
        public const int MaxDistanceError = 13;
        public static readonly byte MaxAngleError = Angle.FromDegreesB(10);

        public struct LookupResult
        {
            public MinutiaPair Pair;
            public int Distance;
        }

        public static void FindMatchingPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar, List<LookupResult> results)
        {
            byte complementaryAngleError = Angle.Complementary(MaxAngleError);
            results.Clear();
            Range range = new Range();

            for (int candidateIndex = 0; candidateIndex < candidateStar.Length; ++candidateIndex)
            {
                NeighborEdge candidateEdge = candidateStar[candidateIndex];
                
                while (range.Begin < probeStar.Length && probeStar[range.Begin].Edge.Length < candidateEdge.Edge.Length - MaxDistanceError)
                    ++range.Begin;
                if (range.End < range.Begin)
                    range.End = range.Begin;
                while (range.End < probeStar.Length && probeStar[range.End].Edge.Length <= candidateEdge.Edge.Length + MaxDistanceError)
                    ++range.End;

                for (int probeIndex = range.Begin; probeIndex < range.End; ++probeIndex)
                {
                    var probeEdge = probeStar[probeIndex];
                    byte referenceDiff = Angle.Difference(probeEdge.Edge.ReferenceAngle, candidateEdge.Edge.ReferenceAngle);
                    if (referenceDiff <= MaxAngleError || referenceDiff >= complementaryAngleError)
                    {
                        byte neighborDiff = Angle.Difference(probeEdge.Edge.NeighborAngle, candidateEdge.Edge.NeighborAngle);
                        if (neighborDiff <= MaxAngleError || neighborDiff >= complementaryAngleError)
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
            if (lengthDelta >= -MaxDistanceError && lengthDelta <= MaxDistanceError)
            {
                byte complementaryAngleError = Angle.Complementary(MaxAngleError);
                byte referenceDelta = Angle.Difference(probe.ReferenceAngle, candidate.ReferenceAngle);
                if (referenceDelta <= MaxAngleError || referenceDelta >= complementaryAngleError)
                {
                    byte neighborDelta = Angle.Difference(probe.NeighborAngle, candidate.NeighborAngle);
                    if (neighborDelta <= MaxAngleError || neighborDelta >= complementaryAngleError)
                        return true;
                }
            }
            return false;
        }

        public static int ComputeHash(EdgeShape edge)
        {
            return (edge.ReferenceAngle / MaxAngleError << 24) + (edge.NeighborAngle / MaxAngleError << 16) + edge.Length / MaxDistanceError;
        }

        public static IEnumerable<int> HashCoverage(EdgeShape edge)
        {
            int minLengthBin = (edge.Length - MaxDistanceError) / MaxDistanceError;
            int maxLengthBin = (edge.Length + MaxDistanceError) / MaxDistanceError;
            int angleBins = Calc.DivRoundUp(256, MaxAngleError);
            int minReferenceBin = Angle.Difference(edge.ReferenceAngle, MaxAngleError) / MaxAngleError;
            int maxReferenceBin = Angle.Add(edge.ReferenceAngle, MaxAngleError) / MaxAngleError;
            int endReferenceBin = (maxReferenceBin + 1) % angleBins;
            int minNeighborBin = Angle.Difference(edge.NeighborAngle, MaxAngleError) / MaxAngleError;
            int maxNeighborBin = Angle.Add(edge.NeighborAngle, MaxAngleError) / MaxAngleError;
            int endNeighborBin = (maxNeighborBin + 1) % angleBins;
            for (int lengthBin = minLengthBin; lengthBin <= maxLengthBin; ++lengthBin)
                for (int referenceBin = minReferenceBin; referenceBin != endReferenceBin; referenceBin = (referenceBin + 1) % angleBins)
                    for (int neighborBin = minNeighborBin; neighborBin != endNeighborBin; neighborBin = (neighborBin + 1) % angleBins)
                        yield return (referenceBin << 24) + (neighborBin << 16) + lengthBin;
        }
    }
}
