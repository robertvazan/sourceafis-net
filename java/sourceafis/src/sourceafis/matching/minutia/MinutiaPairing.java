package sourceafis.matching.minutia;

import sourceafis.extraction.templates.Template;

public class MinutiaPairing implements Cloneable 
{
    int[] ProbeByCandidate;
    int[] CandidateByProbe;
    int[] SupportingEdgesByProbe;
    MinutiaPair[] PairList;
    int PairCount;

    public int getCount() { 
    	return PairCount; 
    	}
    public MinutiaPair getLastAdded() {   
    	return PairList[PairCount - 1]; 
    }
    public void SelectProbe(Template probe)
    {
        CandidateByProbe = new int[probe.Minutiae.length];
        for (int i = 0; i < CandidateByProbe.length; ++i)
            CandidateByProbe[i] = -1;
        SupportingEdgesByProbe = new int[probe.Minutiae.length];
        PairList = new MinutiaPair[probe.Minutiae.length];
        PairCount = 0;
    }

    public void SelectCandidate(Template candidate)
    {
        if (ProbeByCandidate == null || ProbeByCandidate.length < candidate.Minutiae.length)
            ProbeByCandidate = new int[candidate.Minutiae.length];
        for (int i = 0; i < ProbeByCandidate.length; ++i)
            ProbeByCandidate[i] = -1;
    }

    public void Reset()
    {
        for (int i = 0; i < PairCount; ++i)
        {
            ProbeByCandidate[PairList[i].Candidate] = -1;
            CandidateByProbe[PairList[i].Probe] = -1;
            SupportingEdgesByProbe[PairList[i].Probe] = 0;
        }
        PairCount = 0;
    }

    public void Add(MinutiaPair pair)
    {
        ProbeByCandidate[pair.Candidate] = pair.Probe;
        CandidateByProbe[pair.Probe] = pair.Candidate;
        PairList[PairCount] = pair;
        ++PairCount;
    }

    public int GetProbeByCandidate(int candidate)
    {
        return ProbeByCandidate[candidate];
    }

    public int GetCandidateByProbe(int probe)
    {
        return CandidateByProbe[probe];
    }

    public boolean IsProbePaired(int probe)
    {
        return CandidateByProbe[probe] >= 0;
    }

    public boolean IsCandidatePaired(int candidate)
    {
        return ProbeByCandidate[candidate] >= 0;
    }

    public MinutiaPair GetPair(int index)
    {
        return PairList[index];
    }

    public void AddSupportByProbe(int probe)
    {
        ++SupportingEdgesByProbe[probe];
    }

    public int GetSupportByProbe(int probe)
    {
        return SupportingEdgesByProbe[probe];
    }

    /*
     *  Review this later 
     */
    /*public Object Clone()
    {
        MinutiaPairing clone = new MinutiaPairing();
        clone.CandidateByProbe = (int[])CandidateByProbe.clone();
        clone.ProbeByCandidate = (int[])ProbeByCandidate.clone();
        clone.SupportingEdgesByProbe = (int[])SupportingEdgesByProbe.clone();
        clone.PairList = (MinutiaPair[])PairList.clone();
        clone.PairCount = PairCount;
        return clone;
    }*/
}
