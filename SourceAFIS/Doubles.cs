// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	static class Doubles
	{
		public static int Round(double value) { return Math.Round(value, MidpointRounding.ToPositiveInfinity); }
		public static double Sq(double value) { return value * value; }
		public static double Interpolate(double start, double end, double position) { return start + position * (end - start); }
		public static double Interpolate(double bottomleft, double bottomright, double topleft, double topright, double x, double y)
		{
			double left = interpolate(topleft, bottomleft, y);
			double right = interpolate(topright, bottomright, y);
			return interpolate(left, right, x);
		}
		public static double InterpolateExponential(double start, double end, double position) { return Math.pow(end / start, position) * start; }
	}
}
