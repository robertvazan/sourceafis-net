package sourceafis.matching.minutia;

import sourceafis.general.DetailLogger;
import sourceafis.general.IteratorSink;
import sourceafis.general.Predicate;
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

	public void getRoots(ProbeIndex probeIndex, Template candidateTemplate, IteratorSink<MinutiaPair> sink) {
		lookupCounter = 0;
		hashLookup.reset(probeIndex.edgeHash);
		if (!getFilteredRoots(probeIndex, candidateTemplate,
				new Predicate<EdgeShape>() {
					@Override public boolean apply(EdgeShape shape) { return shape.length >= MinEdgeLength; } 
				}, sink))
			return;
		if (!getFilteredRoots(probeIndex, candidateTemplate,
				new Predicate<EdgeShape>() {
					@Override public boolean apply(EdgeShape shape) { return shape.length < MinEdgeLength; } 
				}, sink))
			return;
	}

	public boolean getFilteredRoots(ProbeIndex probeIndex, Template candidateTemplate,
			Predicate<EdgeShape> shapeFilter, IteratorSink<MinutiaPair> sink) {
        if (lookupCounter >= MaxEdgeLookups)
        	return false;
        EdgeConstructor edgeConstructor = new EdgeConstructor();
        for (int step = 1; step < candidateTemplate.minutiae.length; ++step)
            for (int pass = 0; pass < step + 1; ++pass)
                for (int candidateReference = pass; candidateReference < candidateTemplate.minutiae.length; candidateReference += step + 1)
                {
                    int candidateNeighbor = (candidateReference + step) % candidateTemplate.minutiae.length;
                    EdgeShape candidateEdge = edgeConstructor.Construct(candidateTemplate, candidateReference, candidateNeighbor);
                    if (shapeFilter.apply(candidateEdge))
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
