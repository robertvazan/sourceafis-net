using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS
{
    public sealed class FingerprintMatcher
    {
        const int MaxDistanceError = 13;
        static readonly byte MaxAngleError = Angle.FromDegreesB(10);

        public struct MinutiaPair
        {
            public int Probe;
            public int Candidate;
        }

        public struct EdgePair
        {
            public MinutiaPair Reference;
            public MinutiaPair Neighbor;
        }

        class PairInfo
        {
            public MinutiaPair Pair;
            public MinutiaPair Reference;
            public int SupportingEdges;
        }

        internal FingerprintTemplate Template;
        Dictionary<int, List<IndexedEdge>> EdgeHash = new Dictionary<int, List<IndexedEdge>>();
        FingerprintTemplate Candidate;
        PriorityQueueF<EdgePair> PairQueue;

        PairInfo[] PairsByCandidate;
        PairInfo[] PairsByProbe;
        PairInfo[] PairList;
        int PairCount;
        PairInfo LastPair { get { return PairList[PairCount - 1]; } }

        public FingerprintMatcher(FingerprintTemplate template)
        {
            Template = template;
            BuildEdgeHash();

            PairsByProbe = new PairInfo[Template.Minutiae.Count];
            PairList = new PairInfo[Template.Minutiae.Count];
            for (int i = 0; i < PairList.Length; ++i)
                PairList[i] = new PairInfo();
        }

        public double Match(FingerprintTemplate candidate)
        {
            const int maxTriedRoots = 70;
            const int maxTriedTriangles = 7538;

            Candidate = candidate;

            int rootIndex = 0;
            int triangleIndex = 0;
            double bestScore = 0;
            foreach (MinutiaPair root in GetRoots())
            {
                double score = TryRoot(root, candidate);
                if (score > bestScore)
                    bestScore = score;
                ++rootIndex;
                if (rootIndex >= maxTriedRoots)
                    break;
                if (PairCount >= 3)
                {
                    ++triangleIndex;
                    if (triangleIndex >= maxTriedTriangles)
                        break;
                }
            }
            return bestScore;
        }

        class IndexedEdge
        {
            public EdgeShape Shape;
            public int Reference;
            public int Neighbor;
        }

        void BuildEdgeHash()
        {
            for (int referenceMinutia = 0; referenceMinutia < Template.Minutiae.Count; ++referenceMinutia)
                for (int neighborMinutia = 0; neighborMinutia < Template.Minutiae.Count; ++neighborMinutia)
                    if (referenceMinutia != neighborMinutia)
                    {
                        var edge = new IndexedEdge()
                        {
                            Shape = new EdgeShape(Template, referenceMinutia, neighborMinutia),
                            Reference = referenceMinutia,
                            Neighbor = neighborMinutia
                        };
                        foreach (var hash in GetShapeCoverage(edge.Shape))
                        {
                            List<IndexedEdge> list;
                            if (!EdgeHash.TryGetValue(hash, out list))
                                EdgeHash[hash] = list = new List<IndexedEdge>();
                            list.Add(edge);
                        }
                    }
        }

        static IEnumerable<int> GetShapeCoverage(EdgeShape edge)
        {
            int minLengthBin = (edge.Length - MaxDistanceError) / MaxDistanceError;
            int maxLengthBin = (edge.Length + MaxDistanceError) / MaxDistanceError;
            int angleBins = MathEx.DivRoundUp(256, MaxAngleError);
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

        IEnumerable<MinutiaPair> GetRoots()
        {
            const int minEdgeLength = 58;
            const int maxEdgeLookups = 1633;

            int counter = 0;
            var filters = new Predicate<EdgeShape>[]
            {
                shape => shape.Length >= minEdgeLength,
                shape => shape.Length < minEdgeLength
            };
            foreach (var shapeFilter in filters)
            {
                for (int step = 1; step < Candidate.Minutiae.Count; ++step)
                    for (int pass = 0; pass < step + 1; ++pass)
                        for (int candidateReference = pass; candidateReference < Candidate.Minutiae.Count; candidateReference += step + 1)
                        {
                            int candidateNeighbor = (candidateReference + step) % Candidate.Minutiae.Count;
                            var candidateEdge = new EdgeShape(Candidate, candidateReference, candidateNeighbor);
                            if (shapeFilter(candidateEdge))
                            {
                                List<IndexedEdge> matches;
                                if (EdgeHash.TryGetValue(HashShape(candidateEdge), out matches))
                                {
                                    foreach (var match in matches)
                                    {
                                        if (MatchingShapes(match.Shape, candidateEdge))
                                        {
                                            var pair = new MinutiaPair()
                                            {
                                                Probe = match.Reference,
                                                Candidate = candidateReference
                                            };
                                            yield return pair;
                                            ++counter;
                                            if (counter >= maxEdgeLookups)
                                                yield break;
                                        }
                                    }
                                }
                            }
                        }
            }
        }

        static int HashShape(EdgeShape edge)
        {
            return (edge.ReferenceAngle / MaxAngleError << 24) + (edge.NeighborAngle / MaxAngleError << 16) + edge.Length / MaxDistanceError;
        }

        static bool MatchingShapes(EdgeShape probe, EdgeShape candidate)
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

        double TryRoot(MinutiaPair root, FingerprintTemplate candidate)
        {
            CreateRootPairing(root);
            PairQueue = new PriorityQueueF<EdgePair>();
            BuildPairing(candidate);
            return ComputeScore();
        }

        void CreateRootPairing(MinutiaPair root)
        {
            if (PairsByCandidate == null || PairsByCandidate.Length < Candidate.Minutiae.Count)
                PairsByCandidate = new PairInfo[Candidate.Minutiae.Count];
            for (int i = 0; i < PairCount; ++i)
            {
                PairsByProbe[PairList[i].Pair.Probe] = null;
                PairsByCandidate[PairList[i].Pair.Candidate] = null;
            }
            PairsByCandidate[root.Candidate] = PairsByProbe[root.Probe] = PairList[0];
            PairList[0].Pair = root;
            PairCount = 1;
        }

        void BuildPairing(FingerprintTemplate candidate)
        {
            while (true)
            {
                CollectEdges(candidate);
                SkipPaired();
                if (PairQueue.Count == 0)
                    break;
                AddPair(PairQueue.Dequeue());
            }
        }

        void AddPair(EdgePair edge)
        {
            PairsByCandidate[edge.Neighbor.Candidate] = PairsByProbe[edge.Neighbor.Probe] = PairList[PairCount];
            PairList[PairCount].Pair = edge.Neighbor;
            PairList[PairCount].Reference = edge.Reference;
            ++PairCount;
        }

        void SkipPaired()
        {
            while (PairQueue.Count > 0 && (PairsByProbe[PairQueue.Peek().Neighbor.Probe] != null
                || PairsByCandidate[PairQueue.Peek().Neighbor.Candidate] != null))
            {
                EdgePair edge = PairQueue.Dequeue();
                if (PairsByProbe[edge.Neighbor.Probe] != null && PairsByProbe[edge.Neighbor.Probe].Pair.Candidate == edge.Neighbor.Candidate)
                {
                    ++PairsByProbe[edge.Reference.Probe].SupportingEdges;
                    ++PairsByProbe[edge.Neighbor.Probe].SupportingEdges;
                }
            }
        }

        void CollectEdges(FingerprintTemplate candidate)
        {
            var reference = LastPair.Pair;
            var probeNeighbors = Template.EdgeTable[reference.Probe];
            var candidateNeigbors = Candidate.EdgeTable[reference.Candidate];
            var matches = FindMatchingPairs(probeNeighbors, candidateNeigbors);
            foreach (var match in matches)
            {
                var neighbor = match.Pair;
                if (PairsByCandidate[neighbor.Candidate] == null && PairsByProbe[neighbor.Probe] == null)
                    PairQueue.Enqueue(match.Distance, new EdgePair() { Reference = reference, Neighbor = neighbor });
                else if (PairsByProbe[neighbor.Probe] != null && PairsByProbe[neighbor.Probe].Pair.Candidate == neighbor.Candidate)
                {
                    ++PairsByProbe[reference.Probe].SupportingEdges;
                    ++PairsByProbe[neighbor.Probe].SupportingEdges;
                }
            }
        }

        public struct MatchingPair
        {
            public MinutiaPair Pair;
            public int Distance;
        }

        static List<MatchingPair> FindMatchingPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar)
        {
            byte complementaryAngleError = Angle.Complementary(FingerprintMatcher.MaxAngleError);
            var results = new List<MatchingPair>();
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
                            results.Add(new MatchingPair()
                            {
                                Pair = new MinutiaPair()
                                {
                                    Probe = probeEdge.Neighbor,
                                    Candidate = candidateEdge.Neighbor
                                },
                                Distance = candidateEdge.Edge.Length
                            });
                    }
                }
            }
            return results;
        }

        double ComputeScore()
        {
            const int minSupportingEdges = 1;
            const double distanceErrorFlatness = 0.69;
            const double angleErrorFlatness = 0.27;

            const double pairCountFactor = 0.032;
            const double pairFractionFactor = 8.98;
            const double correctTypeFactor = 0.629;
            const double supportedCountFactor = 0.193;
            const double edgeCountFactor = 0.265;
            const double distanceAccuracyFactor = 9.9;
            const double angleAccuracyFactor = 2.79;

            double score = pairCountFactor * PairCount;
            score += pairFractionFactor * (PairCount / (double)Template.Minutiae.Count + PairCount / (double)Candidate.Minutiae.Count) / 2;

            for (int i = 0; i < PairCount; ++i)
            {
                PairInfo pair = PairList[i];
                if (pair.SupportingEdges >= minSupportingEdges)
                    score += supportedCountFactor;
                score += edgeCountFactor * (pair.SupportingEdges + 1);
                if (Template.Minutiae[pair.Pair.Probe].Type == Candidate.Minutiae[pair.Pair.Candidate].Type)
                    score += correctTypeFactor;
            }

            var innerDistanceRadius = Convert.ToInt32(distanceErrorFlatness * FingerprintMatcher.MaxDistanceError);
            var innerAngleRadius = Convert.ToInt32(angleErrorFlatness * FingerprintMatcher.MaxAngleError);

            var distanceErrorSum = 0;
            var angleErrorSum = 0;

            for (int i = 1; i < PairCount; ++i)
            {
                PairInfo pair = PairList[i];
                var probeEdge = new EdgeShape(Template, pair.Reference.Probe, pair.Pair.Probe);
                var candidateEdge = new EdgeShape(Candidate, pair.Reference.Candidate, pair.Pair.Candidate);
                distanceErrorSum += Math.Abs(probeEdge.Length - candidateEdge.Length);
                angleErrorSum += Math.Max(innerDistanceRadius, Angle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle));
                angleErrorSum += Math.Max(innerAngleRadius, Angle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle));
            }

            if (PairCount >= 2)
            {
                var maxDistanceError = FingerprintMatcher.MaxDistanceError * (PairCount - 1);
                score += distanceAccuracyFactor * (maxDistanceError - distanceErrorSum) / maxDistanceError;
                var maxAngleError = FingerprintMatcher.MaxAngleError * (PairCount - 1) * 2;
                score += angleAccuracyFactor * (maxAngleError - angleErrorSum) / maxAngleError;
            }

            return score;
        }
    }
}
