// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Runtime.InteropServices;

namespace SourceAFIS.Engine.Features
{
    // Explicitly request sequential layout for predictable memory usage.
    [StructLayout(LayoutKind.Sequential)]
    readonly struct IndexedEdge
    {
        // Mind the field order. Let the floats in shape get aligned in the whole 16-byte structure.
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
