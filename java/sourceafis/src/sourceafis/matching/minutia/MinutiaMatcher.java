package sourceafis.matching.minutia;

import java.util.List;

import sourceafis.general.IteratorSink;
import sourceafis.general.DetailLogger;
import sourceafis.matching.MatchAnalysis;
import sourceafis.matching.MatchScoring;
import sourceafis.matching.ProbeIndex;
import sourceafis.matching.minutia.EdgeLookup.LookupResult;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;
import sourceafis.meta.ParameterSet;
import sourceafis.templates.Template;

 

public class MinutiaMatcher
{
    @Nested
    public RootPairSelector rootSelector = new RootPairSelector();
    @Nested
    public MinutiaPairing pairing = new MinutiaPairing();
    @Nested
    public EdgeTable edgeTablePrototype = new EdgeTable();
    @Nested
    public EdgeConstructor edgeConstructor = new EdgeConstructor();
    @Nested
    public PairSelector pairSelector = new PairSelector();
    @Nested
    public MatchAnalysis matchAnalysis = new MatchAnalysis();
    @Nested
    public MatchScoring matchScoring = new MatchScoring();
    @Nested
    public EdgeLookup edgeLookup = new EdgeLookup();

    @Parameter(upper=10000)
    public int MaxTriedRoots = 70;
    @Parameter(upper = 10000)
    public int MaxTriedTriangles = 7538;
    public DetailLogger.Hook logger = DetailLogger.off;

    ProbeIndex probe;
    EdgeTable candidateEdges;
    
    public void BuildIndex(Template probe, ProbeIndex index)
    {
        index.template = probe;
        index.edges = ParameterSet.clonePrototype(edgeTablePrototype);
        index.edges.reset(probe);
        index.edgeHash = new EdgeHash(probe, edgeLookup);
    }

    public void SelectProbe(ProbeIndex probe)
    {
        this.probe = probe;
        pairing.selectProbe(probe.template);
        candidateEdges = ParameterSet.clonePrototype(edgeTablePrototype);
    }

    int rootIndex;
    int triangleIndex;
    float bestScore;
    MinutiaPair bestRoot;
    int bestRootIndex;
   
    public float Match(final Template candidate)
    {
        PrepareCandidate(candidate);

        rootIndex = 0;
        triangleIndex = 0;
        bestScore = 0;
        bestRoot = new MinutiaPair();
        bestRootIndex = -1;
     
        rootSelector.getRoots(probe, candidate, new IteratorSink<MinutiaPair>() {
        	@Override public boolean next(MinutiaPair root) {
                float score = TryRoot(root, candidate);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRoot = root;
                    bestRootIndex = rootIndex;
                }
                ++rootIndex;
                if (rootIndex >= MaxTriedRoots)
                    return false;
                if(pairing.getCount() >= 3){
                	++ triangleIndex;
                	if(triangleIndex >= MaxTriedTriangles)
                		return false;
                }
				return true;
			}
		});
        logger.log("score", bestScore);
        logger.log("BestRootIndex", bestRootIndex);
        
        if (bestScore > 0 && logger.isActive())
        {   pairing.Reset(bestRoot);
            BuildPairing(candidate);
            logger.log("bestRoot", bestRoot);
            logger.log("BestPairing",pairing);
        }
        return bestScore;
    }

    void PrepareCandidate(Template candidate)
    {
        pairing.selectCandidate(candidate);
        pairSelector.clear();
        candidateEdges.reset(candidate);
    }
   
    float TryRoot(MinutiaPair root, Template candidate)
    {
        pairing.Reset(root);
        BuildPairing(candidate);
        matchAnalysis.analyze(pairing,edgeLookup, probe.template, candidate);
        return matchScoring.Compute(matchAnalysis);
    }
     
    void BuildPairing(Template candidate)
    {
        while (true)
        {  
          	CollectEdges(candidate);
           	pairSelector.skipPaired(pairing);
            if (pairSelector.getCount() == 0)
                break;
            pairing.add(pairSelector.dequeue());
            
        }
        pairing.log();
    }
    void CollectEdges(Template candidate)
    {
        MinutiaPair reference=pairing.getLastAdded().pair;
        NeighborEdge[] probeNeighbors = probe.edges.Table[reference.probe];
        NeighborEdge[] candidateNeigbors = candidateEdges.Table[reference.candidate];
      
    	List<LookupResult> matches = edgeLookup.FindMatchingPairs(
    			probeNeighbors,candidateNeigbors);
      
        for(LookupResult match : matches)
        {
        	 MinutiaPair neighbor = match.pair;
             if (!pairing.isCandidatePaired(neighbor.candidate) && !pairing.isProbePaired(neighbor.probe))
                 pairSelector.enqueue(new EdgePair(reference, neighbor), match.distance);
             else if (pairing.isProbePaired(neighbor.probe) && pairing.getByProbe(neighbor.probe).pair.candidate == neighbor.candidate)
             {
                 pairing.addSupportByProbe(reference.probe);
                 pairing.addSupportByProbe(neighbor.probe);
             }
         
        }
    }
}
