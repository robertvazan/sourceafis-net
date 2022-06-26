// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Matcher;

namespace SourceAFIS.Engine.Transparency
{
    record ConsistentEdgePair(int ProbeFrom, int ProbeTo, int CandidateFrom, int CandidateTo)
    {
        public ConsistentEdgePair(MinutiaPair pair) : this(pair.ProbeRef, pair.Probe, pair.CandidateRef, pair.Candidate) { }
    }
}
