// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Matcher
{
    class MinutiaPairPool
    {
        MinutiaPair[] pool = new MinutiaPair[1];
        int pooled;
        public MinutiaPair Allocate()
        {
            if (pooled > 0)
            {
                --pooled;
                var pair = pool[pooled];
                pool[pooled] = null;
                return pair;
            }
            else
                return new MinutiaPair();
        }
        public void Release(MinutiaPair pair)
        {
            if (pooled >= pool.Length)
                Array.Resize(ref pool, 2 * pool.Length);
            pair.Probe = 0;
            pair.Candidate = 0;
            pair.ProbeRef = 0;
            pair.CandidateRef = 0;
            pair.Distance = 0;
            pair.SupportingEdges = 0;
            pool[pooled] = pair;
        }
    }
}
