// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Matcher;

namespace SourceAFIS.Transparency
{
    class ConsistentPairingGraph
    {
        public ConsistentMinutiaPair Root;
        public List<ConsistentEdgePair> Tree;
        public List<ConsistentEdgePair> Support;
        public ConsistentPairingGraph(int count, MinutiaPair[] pairs, List<MinutiaPair> support)
        {
            Root = new ConsistentMinutiaPair() { Probe = pairs[0].Probe, Candidate = pairs[0].Candidate };
            Tree = (from p in pairs select new ConsistentEdgePair(p)).Take(count).ToList();
            Support = (from p in support select new ConsistentEdgePair(p)).ToList();
        }
    }
}
