using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public sealed class Calc
    {
        public static int DivRoundUp(int input, int divider)
        {
            return (input + divider - 1) / divider;
        }

        public static int Sq(int value)
        {
            return value * value;
        }

        public static float Sq(float value)
        {
            return value * value;
        }

        public static float Interpolate(float value0, float value1, float fraction)
        {
            return value0 + fraction * (value1 - value0);
        }

        public static float Interpolate(float topLeft, float topRight, float bottomLeft, float bottomRight, PointF fraction)
        {
            float left = Interpolate(bottomLeft, topLeft, fraction.Y);
            float right = Interpolate(bottomRight, topRight, fraction.Y);
            return Interpolate(left, right, fraction.X);
        }

        public static int Interpolate(int index, int count, int range)
        {
            return index * range / count;
        }

        public static float InterpolateExponential(float value0, float value1, float fraction)
        {
            return (float)Math.Pow(value1 / value0, fraction) * value0;
        }

        public static Point Add(Point left, Point right)
        {
            return left + new Size(right);
        }

        public static PointF Add(PointF left, PointF right)
        {
            return left + new SizeF(right);
        }

        public static PointF Multiply(float scale, PointF point)
        {
            return new PointF(scale * point.X, scale * point.Y);
        }

        public static Point Round(PointF point)
        {
            return new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        public static float DistanceSq(PointF point)
        {
            return Sq(point.X) + Sq(point.Y);
        }

        public static int GetArea(Size size)
        {
            return size.Width * size.Height;
        }

        public static void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;
            first = second;
            second = tmp;
        }

        public static void Shuffle<T>(IList<T> collection, Random random)
        {
            for (int i = 0; i < collection.Count - 1; ++i)
            {
                int j = random.Next(collection.Count - i) + i;
                T tmp = collection[i];
                collection[i] = collection[j];
                collection[j] = tmp;
            }
        }

        public static void Shuffle<T>(IList<T> collection)
        {
            Shuffle(collection, new Random(0));
        }

        public static int Compare(int left, int right)
        {
            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }

        public static int ChainCompare(int first, int second)
        {
            if (first != 0)
                return first;
            else
                return second;
        }
    }
}
