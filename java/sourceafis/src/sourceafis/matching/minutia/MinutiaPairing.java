package sourceafis.matching.minutia;

import sourceafis.general.DetailLogger;
import sourceafis.templates.Template;

public class MinutiaPairing implements Cloneable 
{
    PairInfo[] candidateIndex;
    PairInfo[] probeIndex;
    PairInfo[] pairList;
    int pairCount;

    public DetailLogger.Hook logger = DetailLogger.off;

    public int getCount() { 
    	return pairCount; 
    }
    public PairInfo getLastAdded() {   
    	return pairList[pairCount - 1]; 
    }
    public void selectProbe(Template probe)
    {
        probeIndex = new PairInfo[probe.minutiae.length];
        pairList = new PairInfo[probe.minutiae.length];
        for (int i = 0; i < pairList.length; ++i)
           pairList[i] = new PairInfo();
        pairCount = 0;
    }

    public void selectCandidate(Template candidate)
    {
        if (candidateIndex == null || candidateIndex.length < candidate.minutiae.length)
        	candidateIndex = new PairInfo[candidate.minutiae.length];
        else{
        	for (int i = 0; i < candidateIndex.length; ++i)
            candidateIndex[i] = null;
        }
    }

    public void Reset(MinutiaPair root)
    {
        for (int i = 0; i < pairCount; ++i)
        {
            candidateIndex[pairList[i].pair.candidate] = null;
            probeIndex[pairList[i].pair.probe] = null;
            pairList[i].supportingEdges = 0;
        }
        
        candidateIndex[root.candidate] = probeIndex[root.probe] = pairList[0];
        pairList[0].pair = root;
        pairCount = 1;
    }

    public void add(EdgePair edge)
    {
    	 candidateIndex[edge.neighbor.candidate] = probeIndex[edge.neighbor.probe] = pairList[pairCount];
         pairList[pairCount].pair = edge.neighbor;
         pairList[pairCount].reference = edge.reference;
         ++pairCount;
      }

    public PairInfo getByCandidate(int candidate)
    {
        return candidateIndex[candidate];
    }

    public PairInfo getByProbe(int probe)
    {
        return probeIndex[probe];
    }

    public boolean isProbePaired(int probe)
    {
        return probeIndex[probe] != null;
    }

    public boolean isCandidatePaired(int candidate)
    {
        return candidateIndex[candidate] != null;
    }

    public PairInfo getPair(int index)
    {
        return pairList[index];
    }

    public void addSupportByProbe(int probe)
    {
        ++probeIndex[probe].supportingEdges;
    }
    public void log() { logger.log(this); }
   
    public MinutiaPairing clone() {
    	MinutiaPairing clone = new MinutiaPairing();
    	clone.probeIndex = (PairInfo[])probeIndex.clone();
        clone.candidateIndex = (PairInfo[])candidateIndex.clone();
        clone.pairList = new PairInfo[pairList.length];
        for (int i = 0; i < pairList.length; ++i)
        	if (pairList[i] != null)
        		clone.pairList[i] = (PairInfo)pairList[i].clone();
        clone.pairCount = pairCount;
        return clone;
    }
}
