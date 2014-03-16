using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace SourceAFIS.Utils
{
    static class MathEx
    {
        public static int DivRoundUp(int input, int divider)
        {
            return (input + divider - 1) / divider;
        }

        public static int CountBits(uint value)
        {
            int count = 0;
            while (value != 0)
            {
                ++count;
                value &= value - 1;
            }
            return count;
        }

        static byte[] HighestBitCache = CreateHighestBitCache();

        static byte[] CreateHighestBitCache()
        {
            byte[] result = new byte[256];
            for (uint i = 0; i < 256; ++i)
            {
                int highest = 0;
                for (uint j = i; j > 0; j >>= 1)
                    ++highest;
                result[i] = (byte)highest;
            }
            return result;
        }

        public static int HighestBit(uint value)
        {
            if (value < (1u << 8))
                return HighestBitCache[value];
            else if (value < (1u << 16))
                return HighestBitCache[value >> 8];
            else if (value < (1u << 24))
                return HighestBitCache[value >> 16];
            else
                return HighestBitCache[value >> 24];
        }

        public static int Sq(int value)
        {
            return value * value;
        }

        public static double Sq(double value)
        {
            return value * value;
        }

        public static double Interpolate(double value0, double value1, double fraction)
        {
            return value0 + fraction * (value1 - value0);
        }

        public static double Interpolate(double topLeft, double topRight, double bottomLeft, double bottomRight, PointF fraction)
        {
            double left = Interpolate(bottomLeft, topLeft, fraction.Y);
            double right = Interpolate(bottomRight, topRight, fraction.Y);
            return Interpolate(left, right, fraction.X);
        }

        public static int Interpolate(int index, int count, int range)
        {
            return (index * range + count / 2) / count;
        }

        public static double InterpolateExponential(double value0, double value1, double fraction)
        {
            return Math.Pow(value1 / value0, fraction) * value0;
        }

        public static void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;
            first = second;
            second = tmp;
        }

        public static int Compare(int left, int right)
        {
            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }

        public static int Compare(double left, double right)
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

        public static int CompareYX(Point left, Point right)
        {
            return ChainCompare(Compare(left.Y, right.Y), Compare(left.X, right.X));
        }

        public static Point[] ConstructLine(Point from, Point to)
        {
            Point[] result;
            Point relative = to - from;
            if (Math.Abs(relative.X) >= Math.Abs(relative.Y))
            {
                result = new Point[Math.Abs(relative.X) + 1];
                if (relative.X > 0)
                {
                    for (int i = 0; i <= relative.X; ++i)
                        result[i] = new Point(from.X + i, from.Y + Convert.ToInt32(i * (relative.Y / (double)relative.X)));
                }
                else if (relative.X < 0)
                {
                    for (int i = 0; i <= -relative.X; ++i)
                        result[i] = new Point(from.X - i, from.Y - Convert.ToInt32(i * (relative.Y / (double)relative.X)));
                }
                else
                    result[0] = from;
            }
            else
            {
                result = new Point[Math.Abs(relative.Y) + 1];
                if (relative.Y > 0)
                {
                    for (int i = 0; i <= relative.Y; ++i)
                        result[i] = new Point(from.X + Convert.ToInt32(i * (relative.X / (double)relative.Y)), from.Y + i);
                }
                else if (relative.Y < 0)
                {
                    for (int i = 0; i <= -relative.Y; ++i)
                        result[i] = new Point(from.X - Convert.ToInt32(i * (relative.X / (double)relative.Y)), from.Y - i);
                }
                else
                    result[0] = from;
            }
            return result;
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> input, Random random)
        {
            T[] array = input.ToArray();
            for (int i = array.Length - 1; i > 0; --i)
                Swap(ref array[i], ref array[random.Next(i + 1)]);
            return array;
        }
    }
}
