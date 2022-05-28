// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Matcher
{
    class MinutiaPairPool
    {
        MinutiaPair[] Pool = new MinutiaPair[1];
        int Pooled;
        public MinutiaPair Allocate()
        {
            if (Pooled > 0)
            {
                --Pooled;
                var pair = Pool[Pooled];
                Pool[Pooled] = null;
                return pair;
            }
            else
                return new MinutiaPair();
        }
        public void Release(MinutiaPair pair)
        {
            if (Pooled >= Pool.Length)
                Array.Resize(ref Pool, 2 * Pool.Length);
            pair.Probe = 0;
            pair.Candidate = 0;
            pair.ProbeRef = 0;
            pair.CandidateRef = 0;
            pair.Distance = 0;
            pair.SupportingEdges = 0;
            Pool[Pooled] = pair;
        }
    }
}
