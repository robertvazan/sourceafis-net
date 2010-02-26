using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class Matcher
    {
        [Nested]
        public ExhaustiveRootSelector RootSelector = new ExhaustiveRootSelector();
        [Nested]
        public MinutiaPairing Pairing = new MinutiaPairing();
        [Nested]
        public NeighborIterator CandidateNeighbors = new NeighborIterator();
        [Nested]
        public ProbeNeighbors ProbeNeighbors = new ProbeNeighbors();
        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();
        [Nested]
        public PairSelector PairSelector = new PairSelector();
        [Nested]
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();
        [Nested]
        public MatchScoring MatchScoring = new MatchScoring();

        public int MaxTriedRoots = 10000;

        Template Probe;

        public void Prepare(Template probe)
        {
            Probe = probe;
            Pairing.SetProbe(probe);
            ProbeNeighbors.Reset(probe);
        }

        public float Match(Template candidate)
        {
            PrepareCandidate(candidate);

            int rootIndex = 0;
            float bestScore = 0;
            int bestRootIndex = -1;
            foreach (MinutiaPair root in RootSelector.GetRoots(Probe, candidate))
            {
                Logger.Log(this, "Root", root);
                float score = TryRoot(root, candidate);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRootIndex = rootIndex;
                }
                ++rootIndex;
                if (rootIndex >= MaxTriedRoots)
                    break;
            }
            Logger.Log(this, "BestRootIndex", bestRootIndex);
            Logger.Log(this, "Score", bestScore);
            return bestScore;
        }

        void PrepareCandidate(Template candidate)
        {
            Pairing.SetCandidate(candidate);
            PairSelector.Clear();
        }

        float TryRoot(MinutiaPair root, Template candidate)
        {
            Pairing.Reset();
            Pairing.Add(root);
            BuildPairing(candidate);

            MatchAnalysis.Analyze(Pairing, Probe, candidate);
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
            Logger.Log(this, "Pairing", Pairing);
        }

        void CollectEdges(Template candidate)
        {
            foreach (int candidateNeighbor in CandidateNeighbors.GetNeighbors(candidate, Pairing.LastAdded.Candidate))
                if (!Pairing.IsCandidatePaired(candidateNeighbor))
                {
                    EdgeInfo candidateEdge = EdgeConstructor.Construct(candidate, Pairing.LastAdded.Candidate, candidateNeighbor);
                    foreach (int probeNeighbor in ProbeNeighbors.GetMatchingNeighbors(Pairing.LastAdded.Probe, candidateEdge))
                        if (!Pairing.IsProbePaired(probeNeighbor))
                            PairSelector.Enqueue(new MinutiaPair(probeNeighbor, candidateNeighbor), candidateEdge.Length);
                }
        }
    }
}
