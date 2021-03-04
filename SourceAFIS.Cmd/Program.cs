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
			if (args.Length != 1)
				return;
			switch (args[0])
			{
				case "extractor-transparency-stats":
					TransparencyStats.Report(TransparencyStats.ExtractorTable());
					break;
			}
		}
	}
}
