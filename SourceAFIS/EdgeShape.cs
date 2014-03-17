using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Utils;

namespace SourceAFIS
{
    struct EdgeShape
    {
        public readonly int Length;
        public readonly byte ReferenceAngle;
        public readonly byte NeighborAngle;

        const int PolarCacheBits = 8;
        const uint PolarCacheRadius = 1u << PolarCacheBits;
        const uint PolarCacheMask = PolarCacheRadius - 1;

        static readonly int[,] PolarDistance = new int[PolarCacheRadius, PolarCacheRadius];
        static readonly byte[,] PolarAngle = new byte[PolarCacheRadius, PolarCacheRadius];

        static EdgeShape()
        {
            for (int y = 0; y < PolarCacheRadius; ++y)
                for (int x = 0; x < PolarCacheRadius; ++x)
                {
                    PolarDistance[y, x] = Convert.ToInt16(Math.Round(Math.Sqrt(MathEx.Sq(x) + MathEx.Sq(y))));
                    if (y > 0 || x > 0)
                        PolarAngle[y, x] = Angle.AtanB(new Point(x, y));
                    else
                        PolarAngle[y, x] = 0;
                }
        }

        public EdgeShape(FingerprintTemplate template, int reference, int neighbor)
        {
            var vector = template.Minutiae[neighbor].Position - template.Minutiae[reference].Position;
            int quadrant = 0;
            int x = vector.X;
            int y = vector.Y;

            if (y < 0)
            {
                x = -x;
                y = -y;
                quadrant = 128;
            }

            if (x < 0)
            {
                int tmp = -x;
                x = y;
                y = tmp;
                quadrant += 64;
            }

            int shift = MathEx.HighestBit((uint)(x | y) >> PolarCacheBits);

            Length = PolarDistance[y >> shift, x >> shift] << shift;

            var angle = (byte)(PolarAngle[y >> shift, x >> shift] + quadrant);
            ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, angle);
            NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(angle));
        }
    }
}
