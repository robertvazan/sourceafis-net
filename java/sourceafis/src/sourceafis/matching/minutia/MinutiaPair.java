package sourceafis.matching.minutia;
 
public class MinutiaPair{
    public int probe;
    public int candidate;
    /*
     * Empty Constcture 
     */
    public MinutiaPair()
    {
    }
    public MinutiaPair(int probe, int candidate)
    {
        this.probe = probe;
        this.candidate = candidate;
    }
    
    public MinutiaPair clone() {
    	return new MinutiaPair(this.probe,this.candidate);   		 
    }
}
