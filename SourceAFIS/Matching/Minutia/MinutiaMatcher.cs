using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class MinutiaMatcher
    {
        [Nested]
        public RootPairSelector RootSelector = new RootPairSelector();
        [Nested]
        public MinutiaPairing Pairing = new MinutiaPairing();
        [Nested]
        public EdgeTable EdgeTablePrototype = new EdgeTable();
        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();
        [Nested]
        public PairSelector PairSelector = new PairSelector();
        [Nested]
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();
        [Nested]
        public MatchScoring MatchScoring = new MatchScoring();
        [Nested]
        public EdgeLookup EdgeLookup = new EdgeLookup();

        [Parameter(Upper = 10000)]
        public int MaxTriedRoots = 70;
        [Parameter(Upper = 10000)]
        public int MaxTriedTriangles = 7538;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        ProbeIndex Probe;
        EdgeTable CandidateEdges;

        public void BuildIndex(Template probe, ProbeIndex index)
        {
            index.Template = probe;
            index.Edges = ParameterSet.ClonePrototype(EdgeTablePrototype);
            index.Edges.Reset(probe);
            index.EdgeHash = new EdgeHash(probe, EdgeLookup);
        }

        public void SelectProbe(ProbeIndex probe)
        {
            Probe = probe;
            Pairing.SelectProbe(probe.Template);
            CandidateEdges = ParameterSet.ClonePrototype(EdgeTablePrototype);
        }

        public float Match(Template candidate)
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
            Logger.Log("Score", bestScore);
            Logger.Log("BestRootIndex", bestRootIndex);
            if (bestScore > 0 && Logger.IsActive)
            {
                Pairing.Reset(bestRoot);
                BuildPairing(candidate);
                Logger.Log("BestRoot", bestRoot);
                Logger.Log("BestPairing", Pairing);
            }
            return bestScore;
        }

        void PrepareCandidate(Template candidate)
        {
            Pairing.SelectCandidate(candidate);
            PairSelector.Clear();
            CandidateEdges.Reset(candidate);
        }

        float TryRoot(MinutiaPair root, Template candidate)
        {
            Pairing.Reset(root);
            BuildPairing(candidate);

            MatchAnalysis.Analyze(Pairing, EdgeLookup, Probe.Template, candidate);
            return MatchScoring.Compute(MatchAnalysis);
        }

        void BuildPairing(Template candidate)
        {
            while (true)
            {
                CollectEdges(candidate);
                PairSelector.SkipPaired(Pairing);
                if (PairSelector.Count == 0)
                    break;
                Pairing.Add(PairSelector.Dequeue());
            }
            Pairing.Log();
        }

        void CollectEdges(Template candidate)
        {
            var reference = Pairing.LastAdded.Pair;
            var probeNeighbors = Probe.Edges.Table[reference.Probe];
            var candidateNeigbors = CandidateEdges.Table[reference.Candidate];
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
