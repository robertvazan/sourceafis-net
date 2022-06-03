// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Matcher;

namespace SourceAFIS.Engine.Transparency
{
    class ConsistentEdgePair
    {
        public int ProbeFrom;
        public int ProbeTo;
        public int CandidateFrom;
        public int CandidateTo;
        public ConsistentEdgePair(MinutiaPair pair)
        {
            ProbeFrom = pair.ProbeRef;
            ProbeTo = pair.Probe;
            CandidateFrom = pair.CandidateRef;
            CandidateTo = pair.Candidate;
        }
    }
}
