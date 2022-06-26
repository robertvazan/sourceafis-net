// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Engine.Primitives
{
    static class FloatAngle
    {
        public const float Pi = (float)Math.PI;
        public const float Pi2 = (float)(2 * Math.PI);
        public const float HalfPi = (float)(0.5 * Math.PI);

        public static float Add(float start, float delta)
        {
            float angle = start + delta;
            return angle < Pi2 ? angle : angle - Pi2;
        }
        public static float Opposite(float angle) => angle < Pi ? angle + Pi : angle - Pi;
        public static float Distance(float first, float second)
        {
            float delta = Math.Abs(first - second);
            return delta <= Pi ? delta : Pi2 - delta;
        }
        public static float Difference(float first, float second)
        {
            float angle = first - second;
            return angle >= 0 ? angle : angle + Pi2;
        }
        public static float Complementary(float angle)
        {
            float complement = Pi2 - angle;
            return complement < Pi2 ? complement : complement - Pi2;
        }
        public static bool Normalized(float angle) => angle >= 0 && angle < Pi2;
    }
}
