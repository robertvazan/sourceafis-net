package sourceafis.matching.minutia;

public class PairInfo implements Cloneable{
	public MinutiaPair pair;
    public MinutiaPair reference;
    public int supportingEdges;

    public Object clone()
    {
    	PairInfo pairInfo = new PairInfo();
    	if (this.pair != null)
    		pairInfo.pair = this.pair.clone();  
        pairInfo.supportingEdges = this.supportingEdges;
        if (this.reference != null)
        	pairInfo.reference = this.reference.clone();
        return pairInfo;
    }
}
