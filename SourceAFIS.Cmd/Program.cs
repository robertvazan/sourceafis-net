// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using Serilog;

namespace SourceAFIS.Cmd
{
	class Program
	{
		static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(outputTemplate: "{Level:u1} {Message:lj}{NewLine}{Exception}")
				.CreateLogger();
			SampleDataset.Get("fvc2000-2b");
		}
	}
}
