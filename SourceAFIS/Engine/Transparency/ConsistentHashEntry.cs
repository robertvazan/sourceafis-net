// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Transparency
{
    class ConsistentHashEntry
    {
        public int Key;
        public List<IndexedEdge> Edges;
    }
}
