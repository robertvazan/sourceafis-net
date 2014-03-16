using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Matching;

namespace SourceAFIS
{
    public sealed class FingerprintMatcher
    {
        public MinutiaPairing Pairing;
        public PairSelector PairSelector;

        public const int MaxDistanceError = 13;
        public static readonly byte MaxAngleError = Angle.FromDegreesB(10);

        const int MaxTriedRoots = 70;
        const int MaxTriedTriangles = 7538;

        internal FingerprintTemplate Template;
        internal EdgeHash EdgeHash;
        FingerprintTemplate Candidate;

        public FingerprintMatcher(FingerprintTemplate template)
        {
            Template = template;
            EdgeHash = new EdgeHash(template);
        }

        public double Match(FingerprintTemplate candidate)
        {
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
                if (rootIndex >= MaxTriedRoots)
                    break;
                if (Pairing.Count >= 3)
                {
                    ++triangleIndex;
                    if (triangleIndex >= MaxTriedTriangles)
                        break;
                }
            }
            return bestScore;
        }

        IEnumerable<MinutiaPair> GetRoots()
        {
            const int minEdgeLength = 58;
            const int maxEdgeLookups = 1633;

            var hash = new HashLookup(EdgeHash);
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
                                for (var match = hash.Select(candidateEdge); match != null; match = hash.Next())
                                {
                                    var pair = new MinutiaPair(match.Location.Reference, candidateReference);
                                    yield return pair;
                                    ++counter;
                                    if (counter >= maxEdgeLookups)
                                        yield break;
                                }
                            }
                        }
            }
        }

        double TryRoot(MinutiaPair root, FingerprintTemplate candidate)
        {
            Pairing = new MinutiaPairing(Template, candidate, root, Pairing);
            PairSelector = new PairSelector();
            BuildPairing(candidate);
            return ComputeScore();
        }

        void BuildPairing(FingerprintTemplate candidate)
        {
            while (true)
            {
                CollectEdges(candidate);
                PairSelector.SkipPaired(Pairing);
                if (PairSelector.Count == 0)
                    break;
                Pairing.AddPair(PairSelector.Dequeue());
            }
        }

        void CollectEdges(FingerprintTemplate candidate)
        {
            var reference = Pairing.LastPair.Pair;
            var probeNeighbors = Template.EdgeTable[reference.Probe];
            var candidateNeigbors = Candidate.EdgeTable[reference.Candidate];
            var matches = EdgeLookup.FindMatchingPairs(probeNeighbors, candidateNeigbors);
            foreach (var match in matches)
            {
                var neighbor = match.Pair;
                if (!Pairing.IsCandidatePaired(neighbor.Candidate) && !Pairing.IsProbePaired(neighbor.Probe))
                    PairSelector.Enqueue(new EdgePair(reference, neighbor), match.Distance);
                else if (Pairing.IsProbePaired(neighbor.Probe) && Pairing.GetByProbe(neighbor.Probe).Pair.Candidate == neighbor.Candidate)
                {
                    Pairing.AddSupportByProbe(reference.Probe);
                    Pairing.AddSupportByProbe(neighbor.Probe);
                }
            }
        }

        double ComputeScore()
        {
            const int MinSupportingEdges = 1;
            const double DistanceErrorFlatness = 0.69;
            const double AngleErrorFlatness = 0.27;

            const double PairCountFactor = 0.032;
            const double PairFractionFactor = 8.98;
            const double CorrectTypeFactor = 0.629;
            const double SupportedCountFactor = 0.193;
            const double EdgeCountFactor = 0.265;
            const double DistanceAccuracyFactor = 9.9;
            const double AngleAccuracyFactor = 2.79;

            double score = PairCountFactor * Pairing.Count;
            score += PairFractionFactor * (Pairing.Count / (double)Template.Minutiae.Count + Pairing.Count / (double)Candidate.Minutiae.Count) / 2;

            for (int i = 0; i < Pairing.Count; ++i)
            {
                PairInfo pair = Pairing.GetPair(i);
                if (pair.SupportingEdges >= MinSupportingEdges)
                    score += SupportedCountFactor;
                score += EdgeCountFactor * (pair.SupportingEdges + 1);
                if (Template.Minutiae[pair.Pair.Probe].Type == Candidate.Minutiae[pair.Pair.Candidate].Type)
                    score += CorrectTypeFactor;
            }

            var innerDistanceRadius = Convert.ToInt32(DistanceErrorFlatness * FingerprintMatcher.MaxDistanceError);
            var innerAngleRadius = Convert.ToInt32(AngleErrorFlatness * FingerprintMatcher.MaxAngleError);

            var DistanceErrorSum = 0;
            var AngleErrorSum = 0;

            for (int i = 1; i < Pairing.Count; ++i)
            {
                PairInfo pair = Pairing.GetPair(i);
                var probeEdge = new EdgeShape(Template, pair.Reference.Probe, pair.Pair.Probe);
                var candidateEdge = new EdgeShape(Candidate, pair.Reference.Candidate, pair.Pair.Candidate);
                DistanceErrorSum += Math.Abs(probeEdge.Length - candidateEdge.Length);
                AngleErrorSum += Math.Max(innerDistanceRadius, Angle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle));
                AngleErrorSum += Math.Max(innerAngleRadius, Angle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle));
            }

            if (Pairing.Count >= 2)
            {
                var maxDistanceError = FingerprintMatcher.MaxDistanceError * (Pairing.Count - 1);
                score += DistanceAccuracyFactor * (maxDistanceError - DistanceErrorSum) / maxDistanceError;
                var maxAngleError = FingerprintMatcher.MaxAngleError * (Pairing.Count - 1) * 2;
                score += AngleAccuracyFactor * (maxAngleError - AngleErrorSum) / maxAngleError;
            }

            return score;
        }
    }
}
