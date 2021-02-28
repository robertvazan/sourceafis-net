// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	static class Integers
	{
		public static int Sq(int value) { return value * value; }
		public static int RoundUpDiv(int dividend, int divisor) { return (dividend + divisor - 1) / divisor; }
		// https://stackoverflow.com/questions/10439242/count-leading-zeroes-in-an-int32
		// Modified to ignore the sign bit.
		// .NET Core 3 has BitOperations.LeadingZeroCount()
		public static int LeadingZeros(int sx)
		{
			const int numIntBits = sizeof(int) * 8; //compile time constant
			uint x = (uint)sx;
			//do the smearing
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			//count the ones
			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;
			return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
		}
	}
}
