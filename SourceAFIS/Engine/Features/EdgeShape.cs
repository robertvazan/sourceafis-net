// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    readonly struct EdgeShape
    {
        const int PolarCacheBits = 8;
        const int PolarCacheRadius = 1 << PolarCacheBits;

        static readonly int[] PolarDistanceCache = new int[Integers.Sq(PolarCacheRadius)];
        static readonly double[] PolarAngleCache = new double[Integers.Sq(PolarCacheRadius)];

        public readonly int Length;
        public readonly double ReferenceAngle;
        public readonly double NeighborAngle;

        static EdgeShape()
        {
            for (int y = 0; y < PolarCacheRadius; ++y)
            {
                for (int x = 0; x < PolarCacheRadius; ++x)
                {
                    PolarDistanceCache[y * PolarCacheRadius + x] = Doubles.RoundToInt(Math.Sqrt(Doubles.Sq(x) + Doubles.Sq(y)));
                    if (y > 0 || x > 0)
                        PolarAngleCache[y * PolarCacheRadius + x] = DoubleAngle.Atan(x, y);
                    else
                        PolarAngleCache[y * PolarCacheRadius + x] = 0;
                }
            }
        }

        public EdgeShape(int length, double referenceAngle, double neighborAngle)
        {
            Length = length;
            ReferenceAngle = referenceAngle;
            NeighborAngle = neighborAngle;
        }
        public EdgeShape(Minutia reference, Minutia neighbor)
        {
            IntPoint vector = neighbor.Position - reference.Position;
            double quadrant = 0;
            int x = vector.X;
            int y = vector.Y;
            if (y < 0)
            {
                x = -x;
                y = -y;
                quadrant = Math.PI;
            }
            if (x < 0)
            {
                int tmp = -x;
                x = y;
                y = tmp;
                quadrant += DoubleAngle.HalfPi;
            }
            int shift = 32 - (int)Integers.LeadingZeros(((uint)x | (uint)y) >> PolarCacheBits);
            int offset = (y >> shift) * PolarCacheRadius + (x >> shift);
            Length = PolarDistanceCache[offset] << shift;
            double angle = PolarAngleCache[offset] + quadrant;
            ReferenceAngle = DoubleAngle.Difference(reference.Direction, angle);
            NeighborAngle = DoubleAngle.Difference(neighbor.Direction, DoubleAngle.Opposite(angle));
        }
    }
}
