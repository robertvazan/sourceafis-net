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
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();

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
            foreach (MinutiaPair root in RootPairSelector.GetRoots(this, candidate))
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

        double TryRoot(MinutiaPair root, FingerprintTemplate candidate)
        {
            Pairing = new MinutiaPairing(Template, candidate, root, Pairing);
            PairSelector = new PairSelector();
            BuildPairing(candidate);

            MatchAnalysis.Analyze(Pairing, Template, candidate);
            return MatchScoring.Compute(MatchAnalysis);
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
    }
}
