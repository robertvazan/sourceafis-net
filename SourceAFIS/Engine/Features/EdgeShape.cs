// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    readonly struct EdgeShape
    {
        const int PolarCacheBits = 8;
        const int PolarCacheRadius = 1 << PolarCacheBits;

        static readonly short[] PolarDistanceCache = new short[Integers.Sq(PolarCacheRadius)];
        static readonly float[] PolarAngleCache = new float[Integers.Sq(PolarCacheRadius)];

        public readonly short Length;
        public readonly float ReferenceAngle;
        public readonly float NeighborAngle;

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
