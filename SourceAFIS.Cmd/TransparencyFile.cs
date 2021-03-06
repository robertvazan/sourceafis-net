// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SourceAFIS.Cmd
{
	class TransparencyFile
	{
		public static string Extension(String mime)
		{
			switch (mime)
			{
				case "application/cbor":
					return ".cbor";
				case "text/plain":
					return ".txt";
				default:
					return ".dat";
			}
		}
		class FileCollector : FingerprintTransparency
		{
			readonly string Key;
			public readonly List<byte[]> Files = new List<byte[]>();
			public FileCollector(string key) { Key = key; }
			public override bool Accepts(string key) { return Key == key; }
			public override void Take(string key, string mime, byte[] data) { Files.Add(data); }
		}
		public static byte[] Extractor(string key, SampleFingerprint fp)
		{
			var path = fp.Path + Extension(TransparencyStats.ExtractorRow(fp, key).Mime);
			return PersistentCache.Get(Path.Combine("extractor-transparency-files", key), path, () =>
			{
				using (var collector = new FileCollector(key))
				{
					new FingerprintTemplate(fp.Decode());
					return collector.Files[0];
				}
			});
		}
		public static void Extractor(string key)
		{
			foreach (var fp in SampleFingerprint.All)
				Extractor(key, fp);
		}
	}
}
