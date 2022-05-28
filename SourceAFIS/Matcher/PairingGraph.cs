// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;

namespace SourceAFIS.Matcher
{
    class PairingGraph
    {
        public readonly MinutiaPairPool Pool;
        public int Count;
        public MinutiaPair[] Tree = new MinutiaPair[1];
        public MinutiaPair[] ByProbe = new MinutiaPair[1];
        public MinutiaPair[] ByCandidate = new MinutiaPair[1];
        public readonly List<MinutiaPair> SupportEdges = new List<MinutiaPair>();
        public bool SupportEnabled;
        public PairingGraph(MinutiaPairPool pool)
        {
            Pool = pool;
        }
        public void ReserveProbe(FingerprintMatcher matcher)
        {
            int capacity = matcher.Template.Minutiae.Length;
            if (capacity > Tree.Length)
            {
                Tree = new MinutiaPair[capacity];
                ByProbe = new MinutiaPair[capacity];
            }
        }
        public void ReserveCandidate(FingerprintTemplate candidate)
        {
            int capacity = candidate.Minutiae.Length;
            if (ByCandidate.Length < capacity)
                ByCandidate = new MinutiaPair[capacity];
        }
        public void AddPair(MinutiaPair pair)
        {
            Tree[Count] = pair;
            ByProbe[pair.Probe] = pair;
            ByCandidate[pair.Candidate] = pair;
            ++Count;
        }
        public void Support(MinutiaPair pair)
        {
            if (ByProbe[pair.Probe] != null && ByProbe[pair.Probe].Candidate == pair.Candidate)
            {
                ++ByProbe[pair.Probe].SupportingEdges;
                ++ByProbe[pair.ProbeRef].SupportingEdges;
                if (SupportEnabled)
                    SupportEdges.Add(pair);
                else
                    Pool.Release(pair);
            }
            else
                Pool.Release(pair);
        }
        public void Clear()
        {
            for (int i = 0; i < Count; ++i)
            {
                ByProbe[Tree[i].Probe] = null;
                ByCandidate[Tree[i].Candidate] = null;
                // Don't release root, just reset its supporting edge count.
                if (i > 0)
                    Pool.Release(Tree[i]);
                else
                    Tree[0].SupportingEdges = 0;
                Tree[i] = null;
            }
            Count = 0;
            if (SupportEnabled)
            {
                foreach (var pair in SupportEdges)
                    Pool.Release(pair);
                SupportEdges.Clear();
            }
        }
    }
}
