package sourceafis.matching.minutia;

public class PairInfo implements Cloneable{
	public MinutiaPair pair;
    public MinutiaPair reference;
    public int supportingEdges;

    public Object clone()
    {
    	PairInfo pairInfo = new PairInfo();
        pairInfo.pair = this.pair.clone();  
        pairInfo.supportingEdges = this.supportingEdges;
        pairInfo.reference = this.reference.clone();//
        return pairInfo;
    }
}
