// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.IO;
using Serilog;
using SourceAFIS;

namespace SourceAFIS.Cmd
{
	class PersistentCache
	{
		public static readonly string Home = Path.GetFullPath(".cache");
		public static readonly string Output = Path.Combine(Home, "net", FingerprintCompatibility.Version());

		static PersistentCache()
		{
			Log.Information("Cache directory: {Dir}", Home);
			Log.Information("Library version: {Version}", FingerprintCompatibility.Version());
		}
	}
}
