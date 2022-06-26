// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Features
{
    readonly struct IndexedEdge
    {
        public readonly EdgeShape Shape;
        public readonly int Reference;
        public readonly int Neighbor;

        public IndexedEdge(Minutia[] minutiae, int reference, int neighbor)
        {
            Shape = new(minutiae[reference], minutiae[neighbor]);
            Reference = reference;
            Neighbor = neighbor;
        }
    }
}
