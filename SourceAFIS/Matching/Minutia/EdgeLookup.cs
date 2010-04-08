using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class EdgeLookup
    {
        [Parameter(Lower = 0, Upper = 50)]
        public int MaxDistanceError = 6;
        [Parameter(Lower = 1)]
        public byte MaxAngleError = Angle.FromDegreesB(10);

        public struct EdgePair
        {
            public int ProbeIndex;
            public int CandidateIndex;

            public EdgePair(int probe, int candidate)
            {
                ProbeIndex = probe;
                CandidateIndex = candidate;
            }
        }

        List<EdgePair> ReturnList = new List<EdgePair>();

        public List<EdgePair> FindMatchingPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar)
        {
            byte complementaryAngleError = Angle.Complementary(MaxAngleError);
            ReturnList.Clear();
            Range range = new Range();

            for (int candidateIndex = 0; candidateIndex < candidateStar.Length; ++candidateIndex)
            {
                NeighborEdge candidateEdge = candidateStar[candidateIndex];
                
                while (range.Begin < probeStar.Length && probeStar[range.Begin].Edge.Length < candidateEdge.Edge.Length - MaxDistanceError)
                    ++range.Begin;
                if (range.End < range.Begin)
                    range.End = range.Begin;
                while (range.End < probeStar.Length && probeStar[range.End].Edge.Length <= candidateEdge.Edge.Length + MaxDistanceError)
                    ++range.End;

                for (int probeIndex = range.Begin; probeIndex < range.End; ++probeIndex)
                {
                    byte referenceDiff = Angle.Difference(probeStar[probeIndex].Edge.ReferenceAngle, candidateEdge.Edge.ReferenceAngle);
                    if (referenceDiff <= MaxAngleError || referenceDiff >= complementaryAngleError)
                    {
                        byte neighborDiff = Angle.Difference(probeStar[probeIndex].Edge.NeighborAngle, candidateEdge.Edge.NeighborAngle);
                        if (neighborDiff <= MaxAngleError || neighborDiff >= complementaryAngleError)
                        {
                            ReturnList.Add(new EdgePair(probeIndex, candidateIndex));
                        }
                    }
                }
            }

            return ReturnList;
        }
    }
}
