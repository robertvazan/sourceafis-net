package sourceafis.matching.minutia;
 
public class MinutiaPair {
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
}
