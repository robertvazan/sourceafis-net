using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

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

        [Parameter(Upper = 1000)]
        public int MaxTriedRoots = 1000;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        ProbeIndex Probe;
        EdgeTable CandidateEdges;

        public void BuildIndex(Template probe, ProbeIndex index)
        {
            index.Template = probe;
            index.Edges = ParameterSet.ClonePrototype(EdgeTablePrototype);
            index.Edges.Reset(probe);
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
            float bestScore = 0;
            MinutiaPair bestRoot = new MinutiaPair();
            int bestRootIndex = -1;
            foreach (MinutiaPair root in RootSelector.GetRoots(Probe.Template, candidate))
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
            }
            Logger.Log("Score", bestScore);
            Logger.Log("BestRootIndex", bestRootIndex);
            if (bestScore > 0 && Logger.IsActive)
            {
                Pairing.Reset();
                Pairing.Add(bestRoot);
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
            Pairing.Reset();
            Pairing.Add(root);
            BuildPairing(candidate);

            MatchAnalysis.Analyze(Pairing, Probe.Template, candidate);
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
            List<EdgeLookup.EdgePair> edgePairs = EdgeLookup.FindMatchingPairs(
                Probe.Edges.Table[Pairing.LastAdded.Probe], CandidateEdges.Table[Pairing.LastAdded.Candidate]);
            foreach (EdgeLookup.EdgePair edgePair in edgePairs)
            {
                NeighborEdge probeEdge = Probe.Edges.Table[Pairing.LastAdded.Probe][edgePair.ProbeIndex];
                NeighborEdge candidateEdge = CandidateEdges.Table[Pairing.LastAdded.Candidate][edgePair.CandidateIndex];
                if (!Pairing.IsCandidatePaired(candidateEdge.Neighbor) && !Pairing.IsProbePaired(probeEdge.Neighbor))
                    PairSelector.Enqueue(new MinutiaPair(probeEdge.Neighbor, candidateEdge.Neighbor), candidateEdge.Edge.Length);
                else if (Pairing.GetCandidateByProbe(probeEdge.Neighbor) == candidateEdge.Neighbor)
                    Pairing.AddSupportByProbe(probeEdge.Neighbor);
            }
        }
    }
}
