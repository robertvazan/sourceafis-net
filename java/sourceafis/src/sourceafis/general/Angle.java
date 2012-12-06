package sourceafis.general;

public final class Angle {
	public static float PI = (float) Math.PI;
	public static float PI2 = (float) (2 * Math.PI);
	public static final byte PIB = (byte) 128;

	public static final byte B180 = PIB;
	public static final byte B90 = (byte) ((B180 & 0xFF) / 2);
	public static final byte B60 = (byte) ((B180 & 0xFF) / 3);
	public static final byte B45 = (byte) ((B180 & 0xFF) / 4);
	public static final byte B30 = (byte) ((B180 & 0xFF) / 6);
	public static final byte B15 = (byte) ((B180 & 0xFF) / 12);

	public static float FromFraction(float fraction) {
		return fraction * PI2;
	}

	public static float ToFraction(float radians) {
		return radians / PI2;
	}

	public static byte ToByte(float angle) {
		return (byte) Quantize(angle, 256);
	}

	public static float ToFloat(byte angle) {
		return ByBucketCenter(angle, 256);
	}

	public static PointF ToVector(float angle) {
		return new PointF((float) Math.cos(angle), (float) Math.sin(angle));
	}

	public static PointF ToVector(byte angle) {
		return new PointF(Cos(angle), Sin(angle));
	}

	public static float ToOrientation(float direction) {
		if (direction < PI)
			return 2 * direction;
		else
			return 2 * (direction - PI);
	}

	public static byte ToOrientation(byte direction) {
		return (byte) (2 * direction);
	}

	public static byte ToDirection(byte orientation) {
		return (byte) (orientation / 2);
	}

	public static byte FromDegreesB(int degrees) {
		return (byte) ((degrees * 256 + 180) / 360);
	}

	public static int ToDegrees(byte angle) {
		return (angle * 360 + 128) / 256;
	}

	public static float Atan(double x, double y) {
		double result = Math.atan2(y, x);
		if (result < 0)
			result += 2 * Math.PI;
		return (float) result;
	}

	public static float Atan(PointF point) {
		return Atan(point.X, point.Y);
	}

	public static float Atan(Point point) {
		return Atan(point.X, point.Y);
	}

	public static byte AtanB(Point point) {
		return ToByte(Atan(point));
	}

	public static float Atan(Point center, Point point) {
		return Atan(Calc.Difference(point, center));
	}

	public static byte AtanB(Point center, Point point) {
		return ToByte(Atan(center, point));
	}

	static float[] PrecomputedSin = PrecomputeSin();

	static float[] PrecomputeSin() {
		float[] result = new float[256];
		for (int i = 0; i < 256; ++i)
			result[i] = (float) Math.sin(ToFloat((byte) i));
		return result;
	}

	public static float Sin(byte angle) {
		return PrecomputedSin[angle & 0xFF];
	}

	static float[] PrecomputedCos = PrecomputeCos();

	static float[] PrecomputeCos() {
		float[] result = new float[256];
		for (int i = 0; i < 256; ++i)
			result[i] = (float) Math.cos(ToFloat((byte) i));
		return result;
	}

	public static float Cos(byte angle) {
		return PrecomputedCos[angle & 0xFF];
	}

	public static float ByBucketBottom(int bucket, int resolution) {
		return FromFraction((float) bucket / (float) resolution);
	}

	public static float ByBucketTop(int bucket, int resolution) {
		return FromFraction((float) (bucket + 1) / (float) resolution);
	}

	public static float ByBucketCenter(int bucket, int resolution) {
		return FromFraction((float) (2 * bucket + 1) / (float) (2 * resolution));
	}

	public static int Quantize(float angle, int resolution) {
		int result = (int) (ToFraction(angle) * resolution);
		if (result < 0)
			return 0;
		else if (result >= resolution)
			return resolution - 1;
		else
			return result;
	}

	public static int Quantize(byte angle, int resolution) {
		return (angle & 0xFF) * resolution / 256;
	}

	public static float Add(float angle1, float angle2) {
		float result = angle1 + angle2;
		if (result < PI2)
			return result;
		else
			return result - PI2;
	}

	public static byte Add(byte angle1, byte angle2) {
		return (byte) ((angle1 & 0xFF) + (angle2 & 0xFF));
	}

	public static byte Difference(byte angle1, byte angle2) {
		return (byte) ((angle1 & 0xFF) - (angle2 & 0xFF));
	}

	public static byte Distance(byte first, byte second) {
		byte diff = Difference(first, second);
		if ((diff & 0xFF) <= (PIB & 0xFF))
			return diff;
		else
			return Complementary(diff);
	}

	public static byte Complementary(byte angle) {
		return (byte) -(angle & 0xFF);
	}

	public static byte Opposite(byte angle) {
		return (byte) ((angle & 0xFF) + (PIB & 0xFF));
	}

	final static int PolarCacheBits = 8;
	final static int PolarCacheRadius = 1 << PolarCacheBits;
	final int PolarCacheMask = PolarCacheRadius - 1;

	/* struct in c# */
	static class PolarPointB {
		public short Distance;
		public byte Angle;
	}

	static PolarPointB[][] PolarCache;

	static PolarPointB[][] CreatePolarCache() {
		PolarPointB[][] cache = new PolarPointB[PolarCacheRadius][PolarCacheRadius];

		for (int y = 0; y < PolarCacheRadius; ++y)
			for (int x = 0; x < PolarCacheRadius; ++x) {
				// Extra in java as array is not initilized
				cache[y][x] = new PolarPointB();

				cache[y][x].Distance = (short) (Math.round(Math.sqrt(Calc.Sq(x)
						+ Calc.Sq(y))));
				if (y > 0 || x > 0)
					cache[y][x].Angle = Angle.AtanB(new Point(x, y));
				else
					cache[y][x].Angle = 0;
			}
		return cache;
	}

	public static PolarPoint ToPolar(Point point) {
		if (PolarCache == null)
			PolarCache = CreatePolarCache();

		int quadrant = 0;
		int x = point.X;
		int y = point.Y;

		if (y < 0) {
			x = -x;
			y = -y;
			quadrant = 128;
		}

		if (x < 0) {
			int tmp = -x;
			x = y;
			y = tmp;
			quadrant += 64;
		}

		int shift = Calc.HighestBit((x | y) >> PolarCacheBits);

		PolarPointB polarB = PolarCache[y >> shift][x >> shift];
		return new PolarPoint(polarB.Distance << shift,
				(byte) (polarB.Angle + quadrant));
	}
}
