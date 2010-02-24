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
        public EdgeAnalysis CandidateEdge = new EdgeAnalysis();
        [Nested]
        public PairSelector PairSelector = new PairSelector();
        [Nested]
        public MatchAnalysis MatchAnalysis = new MatchAnalysis();
        [Nested]
        public MatchScoring MatchScoring = new MatchScoring();

        public int MaxTriedRoots = 10000;

        public void Prepare(Template probe)
        {
            RootSelector.SetProbe(probe);
            Pairing.SetProbe(probe);
            ProbeNeighbors.Reset(probe);
            MatchAnalysis.SetProbe(probe);
        }

        public float Match(Template candidate)
        {
            PrepareCandidate(candidate);

            int rootIndex = 0;
            float bestScore = 0;
            int bestRootIndex = -1;
            foreach (MinutiaPair root in RootSelector.GetRoots())
            {
                Logger.Log(this, "Root", root);
                float score = TryRoot(root);
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
            RootSelector.SetCandidate(candidate);
            Pairing.SetCandidate(candidate);
            CandidateNeighbors.Reset(candidate);
            CandidateEdge.Template = candidate;
            PairSelector.Clear();
            MatchAnalysis.SetCandidate(candidate);
        }

        float TryRoot(MinutiaPair root)
        {
            Pairing.Reset();
            Pairing.Add(root);
            BuildPairing();

            MatchAnalysis.Analyze(Pairing);
            MatchScoring.Compute(MatchAnalysis);
            return MatchScoring.Score;
        }

        void BuildPairing()
        {
            while (true)
            {
                CollectEdges();
                PairSelector.SkipPaired(Pairing);
                if (PairSelector.Count == 0)
                    break;
                Pairing.Add(PairSelector.Dequeue());
            }
            Logger.Log(this, "Pairing", Pairing);
        }

        void CollectEdges()
        {
            CandidateEdge.ReferenceIndex = Pairing.LastAdded.Candidate;
            foreach (int candidateNeighbor in CandidateNeighbors.GetNeighbors(Pairing.LastAdded.Candidate))
                if (!Pairing.IsCandidatePaired(candidateNeighbor))
                {
                    CandidateEdge.NeighborIndex = candidateNeighbor;
                    CandidateEdge.ComputeAll();
                    foreach (int probeNeighbor in ProbeNeighbors.GetMatchingNeighbors(Pairing.LastAdded.Probe, CandidateEdge))
                        if (!Pairing.IsProbePaired(probeNeighbor))
                            PairSelector.Enqueue(new MinutiaPair(probeNeighbor, candidateNeighbor), CandidateEdge.EdgeLength);
                }
        }
    }
}
