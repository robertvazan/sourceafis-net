package sourceafis.matching.minutia;
 
public class MinutiaPair implements Cloneable {
    public int Probe;
    public int Candidate;
    /*
     * Empty Constcture 
     */
    public MinutiaPair()
    {
    }
    public MinutiaPair(int probe, int candidate)
    {
        Probe = probe;
        Candidate = candidate;
    }
    
    public MinutiaPair clone() {
    	try {
    		return (MinutiaPair)super.clone();
    	} catch (CloneNotSupportedException e) {
    		throw new RuntimeException(e);
    	}
    }
}
