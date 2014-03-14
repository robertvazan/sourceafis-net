using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class FingerprintMatcher
    {
        public RootPairSelector RootSelector = new RootPairSelector();
        public MinutiaPairing Pairing = new MinutiaPairing();
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();
        public PairSelector PairSelector = new PairSelector();
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();
        public MatchScoring MatchScoring = new MatchScoring();
        public EdgeLookup EdgeLookup = new EdgeLookup();

        const int MaxTriedRoots = 70;
        const int MaxTriedTriangles = 7538;

        ProbeIndex Probe;
        FingerprintTemplate Candidate;

        public FingerprintMatcher(FingerprintTemplate template)
        {
            Probe.Template = template;
            Probe.EdgeHash = new EdgeHash(template, EdgeLookup);
            Pairing.SelectProbe(Probe.Template);
        }

        public float Match(FingerprintTemplate candidate)
        {
            PrepareCandidate(candidate);

            int rootIndex = 0;
            int triangleIndex = 0;
            float bestScore = 0;
            MinutiaPair bestRoot = new MinutiaPair();
            int bestRootIndex = -1;
            foreach (MinutiaPair root in RootSelector.GetRoots(Probe, candidate))
            {
                float score = TryRoot(root, candidate);
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

        float TryRoot(MinutiaPair root, FingerprintTemplate candidate)
        {
            Pairing.Reset(root);
            BuildPairing(candidate);

            MatchAnalysis.Analyze(Pairing, EdgeLookup, Probe.Template, candidate);
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
                Pairing.Add(PairSelector.Dequeue());
            }
        }

        void CollectEdges(FingerprintTemplate candidate)
        {
            var reference = Pairing.LastAdded.Pair;
            var probeNeighbors = Probe.Template.EdgeTable[reference.Probe];
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
