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
			if (args.Length < 1)
				return;
			switch (args[0])
			{
				case "footprint":
					TemplateFootprint.Report();
					break;
				case "extractor-transparency-stats":
					TransparencyStats.Report(TransparencyStats.ExtractorTable());
					break;
				case "extractor-transparency-files":
					if (args.Length < 2)
						return;
					TransparencyFile.Extractor(args[1]);
					break;
			}
		}
	}
}
