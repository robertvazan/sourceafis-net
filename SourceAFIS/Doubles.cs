// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	static class Doubles
	{
		public static int RoundToInt(double value) { return (int)Math.Floor(value + 0.5); }
		public static double Sq(double value) { return value * value; }
		public static double Interpolate(double start, double end, double position) { return start + position * (end - start); }
		public static double Interpolate(double bottomleft, double bottomright, double topleft, double topright, double x, double y)
		{
			double left = Interpolate(topleft, bottomleft, y);
			double right = Interpolate(topright, bottomright, y);
			return Interpolate(left, right, x);
		}
		public static double InterpolateExponential(double start, double end, double position) { return Math.Pow(end / start, position) * start; }
	}
}
