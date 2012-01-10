package sourceafis.matching;

import sourceafis.extraction.templates.Template;
import sourceafis.matching.minutia.MinutiaPair;
import sourceafis.matching.minutia.MinutiaPairing;
import sourceafis.meta.Parameter;
 
public  class MatchAnalysis
{
    @Parameter
    public int MinSupportingEdges = 1;

    public int PairCount;
    public int CorrectTypeCount;
    public int SupportedCount;
    public float PairFraction;
    public int EdgeCount;
   
    public void Analyze(MinutiaPairing pairing, Template probe, Template candidate)
    {
        PairCount = pairing.getCount();
        EdgeCount = 0;
        SupportedCount = 0;
        CorrectTypeCount = 0;
        
        for (int i = 0; i < PairCount; ++i)
        {
            MinutiaPair pair = pairing.GetPair(i);
            int support = pairing.GetSupportByProbe(pair.Probe);
            if (support >= MinSupportingEdges)
                ++SupportedCount;
            EdgeCount += support + 1;
            if (probe.Minutiae[pair.Probe].Type == candidate.Minutiae[pair.Candidate].Type)
                ++CorrectTypeCount;
        }
        float probeFraction = PairCount / (float)probe.Minutiae.length;
        float candidateFraction = PairCount / (float)candidate.Minutiae.length;
        PairFraction = (probeFraction + candidateFraction) / 2;
       
      
    }
}