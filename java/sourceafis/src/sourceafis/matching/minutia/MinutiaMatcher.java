package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.List;

import sourceafis.extraction.templates.Template;
import sourceafis.general.DetailLogger;
import sourceafis.matching.MatchAnalysis;
import sourceafis.matching.MatchScoring;
import sourceafis.matching.ProbeIndex;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;

 

public class MinutiaMatcher
{
    @Nested
    public RootPairSelector RootSelector = new RootPairSelector();
    @Nested
    public MinutiaPairing Pairing = new MinutiaPairing();
    @Nested
    public EdgeTable edgeTablePrototype = new EdgeTable();
    @Nested
    public EdgeConstructor EdgeConstructor = new EdgeConstructor();
    @Nested
    public PairSelector PairSelector = new PairSelector();
    @Nested
    public MatchAnalysis MatchAnalysis = new MatchAnalysis();
    @Nested
    public MatchScoring MatchScoring = new MatchScoring();
    @Nested
    public EdgeLookup EdgeLookup = new EdgeLookup();

    @Parameter
    public int MaxTriedRoots = 10000;

    public DetailLogger.Hook logger = DetailLogger.off;

    ProbeIndex Probe;
    EdgeTable CandidateEdges;
    
    public void BuildIndex(Template probe, ProbeIndex index)
    {
        index.Template = probe;
        index.Edges = new EdgeTable();//ParameterSet.ClonePrototype(EdgeTablePrototype);
        index.Edges.Reset(probe);
    }

    public void SelectProbe(ProbeIndex probe)
    {
        Probe = probe;
        Pairing.SelectProbe(probe.Template);
        CandidateEdges = new EdgeTable();;//ParameterSet.ClonePrototype(EdgeTablePrototype);
    }

   
    public float Match(Template candidate)
    {
        PrepareCandidate(candidate);

        int rootIndex = 0;
        float bestScore = 0;
        MinutiaPair bestRoot = null;
     
        for(MinutiaPair root : RootSelector.getRoots(Probe.Template, candidate))
        {
            float score = TryRoot(root, candidate);
            if (score > bestScore)
            {
                bestScore = score;
                bestRoot = root;
            }
            ++rootIndex;
            if (rootIndex >= MaxTriedRoots)
                break;
        }
        logger.log("score", bestScore);
        if (bestScore > 0 && logger.isActive())
            logger.log("root", bestRoot);
        
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
            if (PairSelector.getCount() == 0)
                break;
            Pairing.Add(PairSelector.Dequeue());
            
        }
        
    }
    void CollectEdges(Template candidate)
    {
        List<EdgeLookup.EdgePair> edgePairs = EdgeLookup.FindMatchingPairs(
           Probe.Edges.Table[Pairing.getLastAdded().Probe], CandidateEdges.Table[Pairing.getLastAdded().Candidate]);
      
        for(EdgeLookup.EdgePair edgePair : edgePairs)
        {
            NeighborEdge probeEdge = Probe.Edges.Table[Pairing.getLastAdded().Probe][edgePair.ProbeIndex];
            NeighborEdge candidateEdge = CandidateEdges.Table[Pairing.getLastAdded().Candidate][edgePair.CandidateIndex];
            if (!Pairing.IsCandidatePaired(candidateEdge.Neighbor) && !Pairing.IsProbePaired(probeEdge.Neighbor))
            {
            	PairSelector.Enqueue(new MinutiaPair(probeEdge.Neighbor, candidateEdge.Neighbor), candidateEdge.Edge.Length);
            }
            else if (Pairing.GetCandidateByProbe(probeEdge.Neighbor) == candidateEdge.Neighbor)
                Pairing.AddSupportByProbe(probeEdge.Neighbor);
        }
    }
}
