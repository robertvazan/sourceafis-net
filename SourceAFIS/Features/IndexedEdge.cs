// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Features
{
    class IndexedEdge : EdgeShape
    {
        public readonly int Reference;
        public readonly int Neighbor;

        public IndexedEdge(ImmutableMinutia[] minutiae, int reference, int neighbor)
            : base(minutiae[reference], minutiae[neighbor])
        {
            Reference = reference;
            Neighbor = neighbor;
        }
    }
}
