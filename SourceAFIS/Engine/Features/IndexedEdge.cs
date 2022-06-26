// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Features
{
    readonly struct IndexedEdge
    {
        public readonly EdgeShape Shape;
        public readonly byte Reference;
        public readonly byte Neighbor;

        public IndexedEdge(Minutia[] minutiae, int reference, int neighbor)
        {
            Shape = new(minutiae[reference], minutiae[neighbor]);
            Reference = (byte)reference;
            Neighbor = (byte)neighbor;
        }
    }
}
