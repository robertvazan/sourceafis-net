// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Features;

namespace SourceAFIS.Transparency
{
    class ConsistentHashEntry
    {
        public int Key;
        public List<IndexedEdge> Edges;
    }
}
