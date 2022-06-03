// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Engine.Primitives
{
    static class DoubleAngle
    {
        public const double Pi2 = 2 * Math.PI;
        public const double InvPi2 = 1 / Pi2;
        public const double HalfPi = 0.5 * Math.PI;

        public static DoublePoint ToVector(double angle) => new DoublePoint(Math.Cos(angle), Math.Sin(angle));
        public static double Atan(double x, double y)
        {
            double angle = Math.Atan2(y, x);
            return angle >= 0 ? angle : angle + Pi2;
        }
        public static double Atan(DoublePoint point) => Atan(point.X, point.Y);
        public static double Atan(IntPoint point) => Atan(point.X, point.Y);
        public static double Atan(IntPoint center, IntPoint point) => Atan(point - center);
        public static double ToOrientation(double angle) => angle < Math.PI ? 2 * angle : 2 * (angle - Math.PI);
        public static double FromOrientation(double angle) => 0.5 * angle;
        public static double Add(double start, double delta)
        {
            double angle = start + delta;
            return angle < Pi2 ? angle : angle - Pi2;
        }
        public static double BucketCenter(int bucket, int resolution) => Pi2 * (2 * bucket + 1) / (2 * resolution);
        public static int Quantize(double angle, int resolution)
        {
            int result = (int)(angle * InvPi2 * resolution);
            if (result < 0)
                return 0;
            else if (result >= resolution)
                return resolution - 1;
            else
                return result;
        }
        public static double Opposite(double angle) => angle < Math.PI ? angle + Math.PI : angle - Math.PI;
        public static double Distance(double first, double second)
        {
            double delta = Math.Abs(first - second);
            return delta <= Math.PI ? delta : Pi2 - delta;
        }
        public static double Difference(double first, double second)
        {
            double angle = first - second;
            return angle >= 0 ? angle : angle + Pi2;
        }
        public static double Complementary(double angle)
        {
            double complement = Pi2 - angle;
            return complement < Pi2 ? complement : complement - Pi2;
        }
        public static bool Normalized(double angle) => angle >= 0 && angle < Pi2;
    }
}
