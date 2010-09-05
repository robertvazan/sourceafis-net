using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using System.Reflection;
using System.Linq;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public static class Calc
    {
        public static int DivRoundUp(int input, int divider)
        {
            return (input + divider - 1) / divider;
        }

        public static int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                ++count;
                value &= value - 1;
            }
            return count;
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
            return (index * range + count / 2) / count;
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

        public static Point Difference(Point left, Point right)
        {
            return left - new Size(right);
        }

        public static Point Negate(Point point)
        {
            return new Point(-point.X, -point.Y);
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

        public static int DistanceSq(Point point)
        {
            return Sq(point.X) + Sq(point.Y);
        }

        public static int DistanceSq(Point left, Point right)
        {
            return DistanceSq(Difference(left, right));
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

        public static int Compare(int left, int right)
        {
            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }

        public static int Compare(float left, float right)
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
            Point relative = Difference(to, from);
            if (Math.Abs(relative.X) >= Math.Abs(relative.Y))
            {
                result = new Point[Math.Abs(relative.X) + 1];
                if (relative.X > 0)
                {
                    for (int i = 0; i <= relative.X; ++i)
                        result[i] = new Point(from.X + i, from.Y + Convert.ToInt32(i * (relative.Y / (float)relative.X)));
                }
                else if (relative.X < 0)
                {
                    for (int i = 0; i <= -relative.X; ++i)
                        result[i] = new Point(from.X - i, from.Y - Convert.ToInt32(i * (relative.Y / (float)relative.X)));
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
                        result[i] = new Point(from.X + Convert.ToInt32(i * (relative.X / (float)relative.Y)), from.Y + i);
                }
                else if (relative.Y < 0)
                {
                    for (int i = 0; i <= -relative.Y; ++i)
                        result[i] = new Point(from.X - Convert.ToInt32(i * (relative.X / (float)relative.Y)), from.Y - i);
                }
                else
                    result[0] = from;
            }
            return result;
        }

        public static T DeepClone<T>(this T root) where T : class
        {
            T clone = typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]) as T;
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                if (!fieldInfo.FieldType.IsClass)
                    fieldInfo.SetValue(clone, fieldInfo.GetValue(root));
                else
                    fieldInfo.SetValue(clone, fieldInfo.GetValue(root).DeepClone());
            }
            return clone;
        }

        public static object ShallowClone(this object root)
        {
            object clone = root.GetType().GetConstructor(new Type[0]).Invoke(new object[0]);
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
                fieldInfo.SetValue(clone, fieldInfo.GetValue(root));
            return clone;
        }

        public static IEnumerable<T> CloneItems<T>(this IEnumerable<T> sequence)
            where T : class, ICloneable
        {
            return from item in sequence
                   select item.Clone() as T;
        }

        public static List<T> CloneItems<T>(this List<T> sequence)
            where T : class, ICloneable
        {
            return (from item in sequence
                    select item.Clone() as T).ToList();
        }

        public static void DeepCopyTo(this object source, object target)
        {
            foreach (FieldInfo fieldInfo in source.GetType().GetFields())
            {
                if (!fieldInfo.FieldType.IsClass)
                    fieldInfo.SetValue(target, fieldInfo.GetValue(source));
                else
                    fieldInfo.GetValue(source).DeepCopyTo(fieldInfo.GetValue(target));
            }
        }

        public static bool BeginsWith(this string outer, string inner)
        {
            return outer.Length >= inner.Length && outer.Substring(0, inner.Length) == inner;
        }

        public static float Median(this IEnumerable<float> sequence)
        {
            List<float> sorted = sequence.OrderBy(item => item).ToList();
            return sorted[(sorted.Count - 1) / 2];
        }

        public static void RemoveRange<T>(this List<T> list, int start)
        {
            if (start < list.Count)
                list.RemoveRange(start, list.Count - start);
        }
    }
}
