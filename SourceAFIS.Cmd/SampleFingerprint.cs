// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.IO;

namespace SourceAFIS.Cmd
{
	class SampleFingerprint
	{
		public readonly SampleDataset Dataset;
		public readonly int Id;
		public SampleFingerprint(SampleDataset dataset, int id)
		{
			Dataset = dataset;
			Id = id;
		}
		public String Name { get { return Dataset.Layout.Name(Id); } }
		public byte[] Load() { return File.ReadAllBytes(Path.Combine(SampleDownload.Location(Dataset.Name), Dataset.Layout.Filename(Id))); }
	}
}
