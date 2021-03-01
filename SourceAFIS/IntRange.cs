// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	readonly struct IntRange
	{
		public static readonly IntRange Zero = new IntRange();
		public readonly int Start;
		public readonly int End;

		public int Length { get { return End - Start; } }

		public IntRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		public override string ToString() { return string.Format("{0}..{1}", Start, End); }
	}
}
