// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SourceAFIS.Cmd
{
	class SampleDataset
	{
		public readonly String Name;
		public readonly double Dpi;
		public readonly SampleLayout Layout;
		static double LookupDpi(string dataset)
		{
			switch (dataset)
			{
				case "fvc2002-2b":
					return 569;
				case "fvc2004-3b":
					return 512;
				default:
					return 500;
			}
		}
		SampleDataset(string name)
		{
			Name = name;
			Dpi = LookupDpi(name);
			Layout = SampleLayout.Scan(name);
		}
		static readonly ConcurrentDictionary<string, SampleDataset> Cache = new ConcurrentDictionary<string, SampleDataset>();
		public static SampleDataset Get(string name) { return Cache.GetOrAdd(name, n => new SampleDataset(n)); }
		public static List<SampleDataset> All { get { return SampleDownload.Available.Select(n => Get(n)).ToList(); } }
		public List<SampleFingerprint> Fingerprints { get { return Enumerable.Range(0, Layout.Fingerprints).Select(n => new SampleFingerprint(this, n)).ToList(); } }
	}
}
