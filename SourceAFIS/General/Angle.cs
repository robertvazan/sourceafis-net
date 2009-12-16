using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public sealed class Angle
    {
        public const float PI = (float)Math.PI;
        public const float PI2 = (float)(2 * Math.PI);

        public static float FromFraction(float fraction)
        {
            return fraction * PI2;
        }

        public static float ToFraction(float radians)
        {
            return radians / PI2;
        }

        public static byte ToByte(float angle)
        {
            return (byte)Quantize(angle, 256);
        }

        public static float ToFloat(byte angle)
        {
            return ByBucketCenter(angle, 256);
        }

        public static PointF ToVector(float angle)
        {
            return new PointF((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static PointF ToVector(byte angle)
        {
            return new PointF(Cos(angle), Sin(angle));
        }

        public static float ToOrientation(float direction)
        {
            if (direction < PI)
                return 2 * direction;
            else
                return 2 * (direction - PI);
        }

        public static byte ToOrientation(byte direction)
        {
            return (byte)(2 * direction);
        }

        public static byte ToDirection(byte orientation)
        {
            return (byte)(orientation / 2);
        }

        public static float Atan(double x, double y)
        {
            double result = Math.Atan2(y, x);
            if (result < 0)
                result += 2 * Math.PI;
            return (float)result;
        }

        public static float Atan(PointF point)
        {
            return Atan(point.X, point.Y);
        }

        public static float Atan(Point point)
        {
            return Atan(point.X, point.Y);
        }

        static float[] PrecomputedSin = PrecomputeSin();

        static float[] PrecomputeSin()
        {
            float[] result = new float[256];
            for (int i = 0; i < 256; ++i)
                result[i] = (float)Math.Sin(ToFloat((byte)i));
            return result;
        }

        public static float Sin(byte angle)
        {
            return PrecomputedSin[angle];
        }

        static float[] PrecomputedCos = PrecomputeCos();

        static float[] PrecomputeCos()
        {
            float[] result = new float[256];
            for (int i = 0; i < 256; ++i)
                result[i] = (float)Math.Cos(ToFloat((byte)i));
            return result;
        }

        public static float Cos(byte angle)
        {
            return PrecomputedCos[angle];
        }

        public static float ByBucketBottom(int bucket, int resolution)
        {
            return FromFraction((float)bucket / (float)resolution);
        }

        public static float ByBucketTop(int bucket, int resolution)
        {
            return FromFraction((float)(bucket + 1) / (float)resolution);
        }

        public static float ByBucketCenter(int bucket, int resolution)
        {
            return FromFraction((float)(2 * bucket + 1) / (float)(2 * resolution));
        }

        public static int Quantize(float angle, int resolution)
        {
            int result = (int)(ToFraction(angle) * resolution);
            if (result < 0)
                return 0;
            else if (result >= resolution)
                return resolution - 1;
            else
                return result;
        }

        public static int Quantize(byte angle, int resolution)
        {
            return (int)angle * resolution / 256;
        }

        public static float Add(float angle1, float angle2)
        {
            float result = angle1 + angle2;
            if (result < PI2)
                return result;
            else
                return result - PI2;
        }

        public static byte Add(byte angle1, byte angle2)
        {
            return (byte)(angle1 + angle2);
        }
    }
}
