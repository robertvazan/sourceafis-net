package sourceafis.general;

import java.math.BigDecimal;
import java.math.MathContext;
import java.math.RoundingMode;
import java.util.Arrays;
import java.util.List;
import java.util.Random;

/*
 * 
 */
public final class Calc {
	public static int DivRoundUp(int input, int divider) {
		return (input + divider - 1) / divider;
	}

	public static int CountBits(int value) {
		int count = 0;
		while (value != 0) {
			++count;
			value &= value - 1;
		}
		return count;
	}

	public static int CountBits(short value)// uint
	{
		int count = 0;
		while (value != 0) {
			++count;
			value &= value - 1;
		}
		return count;
	}

	static byte[] HighestBitCache = CreateHighestBitCache();

	static byte[] CreateHighestBitCache() {
		byte[] result = new byte[256];
		for (int i = 0; i < 256; ++i) {
			int highest = 0;
			for (int j = i; j > 0; j >>= 1)
				++highest;
			result[i] = (byte) highest;
		}
		return result;
	}

	public static int HighestBit(int value) {
		if (value < (1 << 8))
			return HighestBitCache[value];
		else if (value < (1 << 16))
			return HighestBitCache[value >> 8];
		else if (value < (1 << 24))
			return HighestBitCache[value >> 16];
		else
			return HighestBitCache[value >> 24];
	}

	public static int Sq(int value) {
		return value * value;
	}

	public static float Sq(float value) {
		return value * value;
	}

	public static float Interpolate(float value0, float value1, float fraction) {
		return value0 + fraction * (value1 - value0);
	}

	public static float Interpolate(float topLeft, float topRight,
			float bottomLeft, float bottomRight, PointF fraction) {
		float left = Interpolate(bottomLeft, topLeft, fraction.Y);
		float right = Interpolate(bottomRight, topRight, fraction.Y);
		return Interpolate(left, right, fraction.X);
	}

	public static int Interpolate(int index, int count, int range) {
		return (index * range + count / 2) / count;
	}

	public static float InterpolateExponential(float value0, float value1,
			float fraction) {
		return (float) Math.pow(value1 / value0, fraction) * value0;
	}

	public static Point Add(Point left, Point right) {
		return Point.add(left, new Size(right)); // left .add new Size(right);
	}

	public static PointF Add(PointF left, PointF right) {
		return PointF.add(left, new SizeF(right));// left + new SizeF(right);
	}

	public static Point Difference(Point left, Point right) {
		if (left == null)
			return right;
		if (right == null)
			return left;
		return Point.minus(left, new Size(right));// left - new Size(right);
	}

	public static Point Negate(Point point) {
		return new Point(-point.X, -point.Y);
	}

	public static PointF Multiply(float scale, PointF point) {
		return new PointF(scale * point.X, scale * point.Y);
	}

	public static Point Multiply(float scale, Point point) {
		return new Point(Calc.toInt32(scale * point.X), Calc.toInt32(scale
				* point.Y));
	}

	/*
	 * Verify Correct Value
	 */
	public static Point Round(PointF point) {

		return new Point(Math.round(point.X), Math.round(point.Y));
	}

	public static float DistanceSq(PointF point) {
		return Sq(point.X) + Sq(point.Y);
	}

	public static int DistanceSq(Point point) {
		return Sq(point.X) + Sq(point.Y);
	}

	public static int DistanceSq(Point left, Point right) {
		return DistanceSq(Difference(left, right));
	}

	public static int GetArea(Size size) {
		return size.Width * size.Height;
	}

	/*
	 * public static <T> void Swap(T first, T second) { T tmp = first; first =
	 * second; second = tmp; }
	 */

	public static int Compare(int left, int right) {
		if (left < right)
			return -1;
		if (left > right)
			return 1;
		return 0;
	}

	public static int Compare(float left, float right) {
		if (left < right)
			return -1;
		if (left > right)
			return 1;
		return 0;
	}

	public static int ChainCompare(int first, int second) {
		if (first != 0)
			return first;
		else
			return second;
	}

	public static int CompareYX(Point left, Point right) {
		return ChainCompare(Compare(left.Y, right.Y), Compare(left.X, right.X));
	}

	public static Point[] ConstructLine(Point from, Point to) {
		Point[] result;
		Point relative = Difference(to, from);
		if (Math.abs(relative.X) >= Math.abs(relative.Y)) {
			result = new Point[Math.abs(relative.X) + 1];
			if (relative.X > 0) {
				for (int i = 0; i <= relative.X; ++i)
					result[i] = new Point(from.X + i, from.Y
							+ (int) (i * (relative.Y / (float) relative.X)));
			} else if (relative.X < 0) {
				for (int i = 0; i <= -relative.X; ++i)
					result[i] = new Point(from.X - i, from.Y
							- (int) (i * (relative.Y / (float) relative.X)));
			} else
				result[0] = from;
		} else {
			result = new Point[Math.abs(relative.Y) + 1];
			if (relative.Y > 0) {
				for (int i = 0; i <= relative.Y; ++i)
					result[i] = new Point(from.X
							+ (int) (i * (relative.X / (float) relative.Y)),
							from.Y + i);
			} else if (relative.Y < 0) {
				for (int i = 0; i <= -relative.Y; ++i)
					result[i] = new Point(from.X
							- (int) (i * (relative.X / (float) relative.Y)),
							from.Y - i);
			} else
				result[0] = from;
		}
		return result;
	}

	/*
	 * public static T DeepClone<T>(this T root) where T : class { T clone =
	 * root.GetType().GetConstructor(new Type[0]).Invoke(new object[0]) as T;
	 * foreach (FieldInfo fieldInfo in root.GetType().GetFields()) { if
	 * (!fieldInfo.FieldType.IsClass) fieldInfo.SetValue(clone,
	 * fieldInfo.GetValue(root)); else fieldInfo.SetValue(clone,
	 * fieldInfo.GetValue(root).DeepClone()); } return clone; }
	 */

	/*
	 * public static object ShallowClone(this object root) { object clone =
	 * root.GetType().GetConstructor(new Type[0]).Invoke(new object[0]); foreach
	 * (FieldInfo fieldInfo in root.GetType().GetFields())
	 * fieldInfo.SetValue(clone, fieldInfo.GetValue(root)); return clone; }
	 * 
	 * public static IEnumerable<T> CloneItems<T>(this IEnumerable<T> sequence)
	 * where T : class, ICloneable { return from item in sequence select
	 * item.Clone() as T; }
	 * 
	 * public static List<T> CloneItems<T>(this List<T> sequence) where T :
	 * class, ICloneable { return (from item in sequence select item.Clone() as
	 * T).ToList(); }
	 * 
	 * public static void DeepCopyTo(this object source, object target) {
	 * foreach (FieldInfo fieldInfo in source.GetType().GetFields()) { if
	 * (!fieldInfo.FieldType.IsClass) fieldInfo.SetValue(target,
	 * fieldInfo.GetValue(source)); else
	 * fieldInfo.GetValue(source).DeepCopyTo(fieldInfo.GetValue(target)); } }
	 */
	public static boolean BeginsWith(String outer, String inner) {
		return outer.startsWith(inner);
	}

	/*
	 * public static float Median(this IEnumerable<float> sequence) {
	 * List<float> sorted = sequence.OrderBy(item => item).ToList(); return
	 * sorted[(sorted.Count - 1) / 2]; }
	 */
	/*
	 * public static void RemoveRange(List list, int start) {
	 * 
	 * if (start < list.size()) list.subList(start, list.size() -
	 * start).clear();
	 * 
	 * }
	 */
	/*
	 * static public uint ReverseBitsInBytes(uint word) { uint phase1 = (word >>
	 * 4) & 0x0f0f0f0f | (word << 4) & 0xf0f0f0f0; uint phase2 = (phase1 >> 2) &
	 * 0x33333333 | (phase1 << 2) & 0xcccccccc; return (phase2 >> 1) &
	 * 0x55555555 | (phase2 << 1) & 0xaaaaaaaa; }
	 */

	public static int toInt32(float number) {
		return new BigDecimal(number,
				new MathContext(0, RoundingMode.HALF_EVEN)).intValue();
	}

	public static byte toByte(float number) {
		return new BigDecimal(number,
				new MathContext(0, RoundingMode.HALF_EVEN)).byteValue();
	}

	public static <T> List<T> Shuffle(List<T> input, Random random) {
		@SuppressWarnings("unchecked")
		T[] array = (T[]) input.toArray();
		for (int i = array.length - 1; i > 0; --i) {
			int r = random.nextInt(i + 1);
			T tmp = array[i];
			array[i] = array[r];
			array[r] = tmp;
		}
		return Arrays.asList(array);
	}

	public static boolean areEqual(Object o1, Object o2) {
		return (o1 == null && o2 == null) || (o1 != null && o1.equals(o2));
	}
}
