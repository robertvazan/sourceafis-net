using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Matching;

namespace SourceAFIS
{
    public sealed class FingerprintMatcher
    {
        public MinutiaPairing Pairing = new MinutiaPairing();
        public PairSelector PairSelector = new PairSelector();
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();

        const int MaxTriedRoots = 70;
        const int MaxTriedTriangles = 7538;

        internal FingerprintTemplate Template;
        internal EdgeHash EdgeHash;
        FingerprintTemplate Candidate;
        List<EdgeLookup.LookupResult> EdgeMatches = new List<EdgeLookup.LookupResult>();

        public FingerprintMatcher(FingerprintTemplate template)
        {
            Template = template;
            EdgeHash = new EdgeHash(template);
            Pairing.SelectProbe(Template);
        }

        public double Match(FingerprintTemplate candidate)
        {
            PrepareCandidate(candidate);

            int rootIndex = 0;
            int triangleIndex = 0;
            double bestScore = 0;
            MinutiaPair bestRoot = new MinutiaPair();
            int bestRootIndex = -1;
            foreach (MinutiaPair root in RootPairSelector.GetRoots(this, candidate))
            {
                double score = TryRoot(root, candidate);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRoot = root;
                    bestRootIndex = rootIndex;
                }
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

        void PrepareCandidate(FingerprintTemplate candidate)
        {
            Pairing.SelectCandidate(candidate);
            PairSelector.Clear();
            Candidate = candidate;
        }

        double TryRoot(MinutiaPair root, FingerprintTemplate candidate)
        {
            Pairing.ResetPairing(root);
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
            EdgeLookup.FindMatchingPairs(probeNeighbors, candidateNeigbors, EdgeMatches);
            foreach (var match in EdgeMatches)
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
