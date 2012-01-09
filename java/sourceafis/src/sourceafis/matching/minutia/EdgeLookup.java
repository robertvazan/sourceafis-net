package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.List;

import sourceafis.general.Angle;
import sourceafis.general.Range;
import sourceafis.meta.Parameter;
 

   public class EdgeLookup
    {
        @Parameter
        public int MaxDistanceError = 6;
        @Parameter
        public byte MaxAngleError = Angle.FromDegreesB(10);

        public class EdgePair
        {
            public int ProbeIndex;
            public int CandidateIndex;

            public EdgePair(int probe, int candidate)
            {
                ProbeIndex = probe;
                CandidateIndex = candidate;
            }
        }

        List<EdgePair> ReturnList = new ArrayList<EdgePair>();
       
        public List<EdgePair> FindMatchingPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar)
        {
        	 //convert to int as java doesn't have unsigned 
        	int complementaryAngleError = Angle.Complementary(MaxAngleError)& 0xFF;
        	ReturnList.clear();
            Range range = new Range();
            for (int candidateIndex = 0; candidateIndex < candidateStar.length; ++candidateIndex)
            {
                NeighborEdge candidateEdge = candidateStar[candidateIndex];
       
                
                while (range.Begin < probeStar.length && probeStar[range.Begin].Edge.Length < candidateEdge.Edge.Length - MaxDistanceError)
                    ++range.Begin;
               
                if (range.End < range.Begin)
                		range.End = range.Begin;
                
                while (range.End < probeStar.length && probeStar[range.End].Edge.Length <= candidateEdge.Edge.Length + MaxDistanceError)
                    ++range.End;
               
                for (int probeIndex = range.Begin; probeIndex < range.End; ++probeIndex)
                {
                     
                	//convert to int as java doesn't have unsigned
                    int referenceDiff = (Angle.Difference(probeStar[probeIndex].Edge.ReferenceAngle, candidateEdge.Edge.ReferenceAngle)&0xFF);
                    //  if (referenceDiff <= MaxAngleError || referenceDiff >= complementaryAngleError)
                    if (referenceDiff <= (MaxAngleError&0xFF) || referenceDiff >= complementaryAngleError)
                    {
                        int neighborDiff = 0xFF & Angle.Difference(probeStar[probeIndex].Edge.NeighborAngle, candidateEdge.Edge.NeighborAngle);
                        if (neighborDiff <= (MaxAngleError&0xFF) || neighborDiff >= complementaryAngleError)
                        {
                            ReturnList.add(new EdgePair(probeIndex, candidateIndex));
                        }
                    }
                }
            }
            
            return ReturnList;
        }
    }
 
