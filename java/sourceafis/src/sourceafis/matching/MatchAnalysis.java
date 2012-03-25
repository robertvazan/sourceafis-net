package sourceafis.matching;

import sourceafis.general.Angle;
import sourceafis.matching.minutia.EdgeLookup;
import sourceafis.matching.minutia.EdgeShape;
import sourceafis.matching.minutia.MinutiaPairing;
import sourceafis.matching.minutia.PairInfo;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;
import sourceafis.templates.Template;
import sourceafis.matching.minutia.EdgeConstructor; 
public  class MatchAnalysis
{
    @Parameter(lower=0 , upper= 5)
    public int MinSupportingEdges = 1;
    @Parameter
    public float DistanceErrorFlatness = 0.69f;
    @Parameter
    public float AngleErrorFlatness = 0.27f;
    @Nested
    public EdgeConstructor edgeConstructor = new EdgeConstructor();

    public int maxDistanceError;
    public byte maxAngleError;
    public int pairCount;
    public int correctTypeCount;
    public int supportedCount;
    public float pairFraction;
    public int edgeCount;
    public int distanceErrorSum;
    public int angleErrorSum;
   
    public void analyze(MinutiaPairing pairing,EdgeLookup lookup,Template probe, Template candidate)
    {
    	  maxDistanceError = lookup.MaxDistanceError;
          maxAngleError = lookup.MaxAngleError;
          int innerDistanceRadius = Math.round(DistanceErrorFlatness * maxDistanceError);
          int innerAngleRadius = Math.round(AngleErrorFlatness * maxAngleError);

          pairCount = pairing.getCount();

          edgeCount = 0;
          supportedCount = 0;
          correctTypeCount = 0;
          distanceErrorSum = 0;
          angleErrorSum = 0;

        pairCount = pairing.getCount();
        edgeCount = 0;
        supportedCount = 0;
        correctTypeCount = 0;
        
        for (int i = 0; i < pairCount; ++i)
        {
            PairInfo pair = pairing.getPair(i);
            if (pair.supportingEdges >= MinSupportingEdges)
                ++supportedCount;
            edgeCount += pair.supportingEdges + 1;
            if (probe.minutiae[pair.pair.probe].Type == candidate.minutiae[pair.pair.candidate].Type)
                ++correctTypeCount;
            if (i > 0)
            {
                EdgeShape probeEdge = edgeConstructor.Construct(probe, pair.reference.probe, pair.pair.probe);
                EdgeShape candidateEdge = edgeConstructor.Construct(candidate, pair.reference.candidate, pair.pair.candidate);
                distanceErrorSum += Math.abs(probeEdge.length - candidateEdge.length);
                angleErrorSum += Math.max(innerDistanceRadius, Angle.Distance(probeEdge.referenceAngle, candidateEdge.referenceAngle) & 0xFF);
                angleErrorSum += Math.max(innerAngleRadius, Angle.Distance(probeEdge.neighborAngle, candidateEdge.neighborAngle) & 0xFF);
            }
            
        }
        float probeFraction = pairCount / (float)probe.minutiae.length;
        float candidateFraction = pairCount / (float)candidate.minutiae.length;
        pairFraction = (probeFraction + candidateFraction) / 2;
       
      
    }
}