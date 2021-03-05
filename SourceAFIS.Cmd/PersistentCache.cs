// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Serilog;

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

		static byte[] Gzip(byte[] data)
		{
			using (var buffer = new MemoryStream())
			{
				using (var gzip = new GZipStream(buffer, CompressionMode.Compress))
					gzip.Write(data);
				return buffer.ToArray();
			}
		}
		static byte[] Gunzip(byte[] compressed)
		{
			using (var buffer = new MemoryStream())
			{
				using (var input = new MemoryStream(compressed))
					using (var gzip = new GZipStream(input, CompressionMode.Decompress))
						gzip.CopyTo(buffer);
				return buffer.ToArray();
			}
		}
		static readonly HashSet<string> Reported = new HashSet<string>();
		public static T Get<T>(string category, string identity, Func<T> supplier)
		{
			var path = Path.Combine(Output, category, identity) + ".cbor.gz";
			if (File.Exists(path))
				return SerializationUtils.Deserialize<T>(Gunzip(File.ReadAllBytes(path)));
			lock (Reported)
			{
				if (!Reported.Contains(category))
				{
					Reported.Add(category);
					Log.Information("Computing {Category}...", category);
				}
			}
			T fresh = supplier();
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, Gzip(SerializationUtils.Serialize(fresh)));
			return fresh;
		}
		public static T Get<T>(string category, SampleFingerprint fp, Func<T> supplier) { return Get<T>(category, Path.Combine(fp.Dataset.Name, fp.Name), supplier); }
	}
}
