// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	readonly struct IntRange
	{
		public static readonly IntRange Zero = new IntRange();
		public readonly int Begin;
		public readonly int End;

		public int Length { get { return End - Begin; } }

		public IntRange(int begin, int end)
		{
			Begin = begin;
			End = end;
		}

		public override string ToString() { return string.Format("{0}..{1}", Begin, End); }
	}
}
