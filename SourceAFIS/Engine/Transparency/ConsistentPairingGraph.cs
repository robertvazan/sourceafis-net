// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Engine.Matcher;

namespace SourceAFIS.Engine.Transparency
{
    record ConsistentPairingGraph(ConsistentMinutiaPair Root, List<ConsistentEdgePair> Tree, List<ConsistentEdgePair> Support)
    {
        public ConsistentPairingGraph(int count, MinutiaPair[] pairs, List<MinutiaPair> support)
            : this(
                new ConsistentMinutiaPair(pairs[0].Probe, pairs[0].Candidate),
                (from p in pairs select new ConsistentEdgePair(p)).Take(count).ToList(),
                (from p in support select new ConsistentEdgePair(p)).ToList())
        {
        }
    }
}
