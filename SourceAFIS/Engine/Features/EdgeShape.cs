// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Runtime.InteropServices;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    // No padding, so that edge structs can put fields where padding would otherwise be.
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    readonly struct EdgeShape
    {
        const int PolarCacheBits = 8;
        const int PolarCacheRadius = 1 << PolarCacheBits;

        static readonly short[] PolarDistanceCache = new short[Integers.Sq(PolarCacheRadius)];
        static readonly float[] PolarAngleCache = new float[Integers.Sq(PolarCacheRadius)];

        // Mind the field order. Floats first to ensure they are aligned despite 2-byte struct packing.
        public readonly float ReferenceAngle;
        public readonly float NeighborAngle;
        public readonly short Length;

        // This will only be the case with sequential layout and 2-byte packing.
        public const int Memory = 2 * sizeof(float) + sizeof(short);

        static EdgeShape()
        {
            for (int y = 0; y < PolarCacheRadius; ++y)
            {
                for (int x = 0; x < PolarCacheRadius; ++x)
                {
                    PolarDistanceCache[y * PolarCacheRadius + x] = (short)Doubles.RoundToInt(Math.Sqrt(Doubles.Sq(x) + Doubles.Sq(y)));
                    if (y > 0 || x > 0)
                        PolarAngleCache[y * PolarCacheRadius + x] = (float)DoubleAngle.Atan(x, y);
                    else
                        PolarAngleCache[y * PolarCacheRadius + x] = 0;
                }
            }
        }

        public EdgeShape(int length, float referenceAngle, float neighborAngle)
        {
            Length = (short)length;
            ReferenceAngle = referenceAngle;
            NeighborAngle = neighborAngle;
        }
        public EdgeShape(Minutia reference, Minutia neighbor)
        {
            var vector = neighbor.Position - reference.Position;
            float quadrant = 0;
            int x = vector.X;
            int y = vector.Y;
            if (y < 0)
            {
                x = -x;
                y = -y;
                quadrant = FloatAngle.Pi;
            }
            if (x < 0)
            {
                int tmp = -x;
                x = y;
                y = tmp;
                quadrant += FloatAngle.HalfPi;
            }
            int shift = 32 - (int)Integers.LeadingZeros(((uint)x | (uint)y) >> PolarCacheBits);
            int offset = (y >> shift) * PolarCacheRadius + (x >> shift);
            Length = (short)(PolarDistanceCache[offset] << shift);
            float angle = PolarAngleCache[offset] + quadrant;
            ReferenceAngle = FloatAngle.Difference(reference.Direction, angle);
            NeighborAngle = FloatAngle.Difference(neighbor.Direction, FloatAngle.Opposite(angle));
        }
    }
}
