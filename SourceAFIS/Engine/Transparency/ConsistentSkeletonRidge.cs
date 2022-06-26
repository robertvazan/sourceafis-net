// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Transparency
{
    record ConsistentSkeletonRidge(int Start, int End, IList<IntPoint> Points)
    {
    }
}
