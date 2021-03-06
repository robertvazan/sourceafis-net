// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    class MinutiaPair
    {
        public int Probe;
        public int Candidate;
        public int ProbeRef;
        public int CandidateRef;
        public int Distance;
        public int SupportingEdges;

        public override string ToString() { return string.Format("{0}<->{1} @ {2}<->{3} #{4}", Probe, Candidate, ProbeRef, CandidateRef, SupportingEdges); }
    }
}
