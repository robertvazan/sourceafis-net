package sourceafis.matching.minutia;

import sourceafis.general.DetailLogger;
import sourceafis.matching.ProbeIndex;
import sourceafis.meta.Parameter;
import sourceafis.templates.Template;

public class RootPairSelector{
	@Parameter(lower=5 , upper = 300)
	public int MinEdgeLength = 58;
	@Parameter (lower = 5 , upper = 100000)
	public int MaxEdgeLookups = 1633;
	
	HashLookup hashLookup = new HashLookup();
	int lookupCounter;

    public DetailLogger.Hook logger = DetailLogger.off;

    public interface PairSink {
    	boolean next(MinutiaPair root);
    }
    
	public void getRoots(ProbeIndex probeIndex, Template candidateTemplate, PairSink sink) {
	      lookupCounter = 0;
          hashLookup.reset(probeIndex.edgeHash);
          if (!getFilteredRoots(probeIndex, candidateTemplate, 0, sink))
        	  return;
          if (!getFilteredRoots(probeIndex, candidateTemplate, 1, sink))
        	  return;
 	}

	private boolean predicate(EdgeShape shape,int mode){
	   	if(mode==0){
		   return  shape.length >= MinEdgeLength;	
		}else{ 
		   return shape.length < MinEdgeLength;	
		}
	}
    
	public boolean getFilteredRoots(ProbeIndex probeIndex, Template candidateTemplate, int mode, PairSink sink) {
        if (lookupCounter >= MaxEdgeLookups)
        	return false;
        EdgeConstructor edgeConstructor = new EdgeConstructor();
        for (int step = 1; step < candidateTemplate.Minutiae.length; ++step)
            for (int pass = 0; pass < step + 1; ++pass)
                for (int candidateReference = pass; candidateReference < candidateTemplate.Minutiae.length; candidateReference += step + 1)
                {
                    int candidateNeighbor = (candidateReference + step) % candidateTemplate.Minutiae.length;
                    EdgeShape candidateEdge = edgeConstructor.Construct(candidateTemplate, candidateReference, candidateNeighbor);
                    if (predicate(candidateEdge, mode))
                    {
                        for (IndexedEdge match = hashLookup.select(candidateEdge); match != null; match = hashLookup.next())
                        {
                        	MinutiaPair pair = new MinutiaPair(match.location.reference, candidateReference);
                            if (logger.isActive())
                            	logger.log(pair);
                            if (!sink.next(pair))
                            	return false;
                            ++lookupCounter;
                            if (lookupCounter >= MaxEdgeLookups)
                            	return false;
                        }
                    }
                }
        return true;
    }
}
