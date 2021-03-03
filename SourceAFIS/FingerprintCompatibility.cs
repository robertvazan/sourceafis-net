// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	public static class FingerprintCompatibility
	{
		public static String Version() { return typeof(FingerprintCompatibility).Assembly.GetName().Version.ToString(3); }
	}
}
