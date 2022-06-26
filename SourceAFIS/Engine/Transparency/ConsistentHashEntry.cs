// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Transparency
{
    record ConsistentHashEntry(int Key, List<IndexedEdge> Edges)
    {
    }
}
