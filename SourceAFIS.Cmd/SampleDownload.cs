// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using Serilog;

namespace SourceAFIS.Cmd
{
	static class SampleDownload
	{
		public static readonly string[] Available = new[]
		{
			"fvc2000-1b",
			"fvc2000-2b",
			"fvc2000-3b",
			"fvc2000-4b",
			"fvc2002-1b",
			"fvc2002-2b",
			"fvc2002-3b",
			"fvc2002-4b",
			"fvc2004-1b",
			"fvc2004-2b",
			"fvc2004-3b",
			"fvc2004-4b"
		};

		static string Url(String dataset)
		{
			switch (dataset)
			{
				case "fvc2000-1b":
					return "https://cdn.machinezoo.com/h/5JV6zPvfEdLsVByxgQfx8mZzp_GK1uB4gMbpTRVk3vI/fvc2000-1b-png.zip";
				case "fvc2000-2b":
					return "https://cdn.machinezoo.com/h/7AnG1QKQFIO_SPE2QfQOOh2niRt5SwIWKTBNMODhy9k/fvc2000-2b-png.zip";
				case "fvc2000-3b":
					return "https://cdn.machinezoo.com/h/zYWii7GxCYODyMQXL_0V5XYBAeGDL0ZyAn9ueGJxKWo/fvc2000-3b-png.zip";
				case "fvc2000-4b":
					return "https://cdn.machinezoo.com/h/iJwQy6OqL8GfKmb-8CahSPPe-TSO1Il-84ECZbzH7BU/fvc2000-4b-png.zip";
				case "fvc2002-1b":
					return "https://cdn.machinezoo.com/h/27Ywz3grZYFSPdVGhdksFPJ7LMLH5XXXzXdLoi6OmO0/fvc2002-1b-png.zip";
				case "fvc2002-2b":
					return "https://cdn.machinezoo.com/h/PN2JaZ2IsHWGcCakDBFXM4bxscockSjqTVmISrFYaes/fvc2002-2b-png.zip";
				case "fvc2002-3b":
					return "https://cdn.machinezoo.com/h/CjrVS4yjjF0EovpOMU6onMAdUbw86LF9Q9ZZAaqbx1A/fvc2002-3b-png.zip";
				case "fvc2002-4b":
					return "https://cdn.machinezoo.com/h/XMlAqAkmJBOnb-u2EpoASAi4tZv8-06J3YPFyrYD_AM/fvc2002-4b-png.zip";
				case "fvc2004-1b":
					return "https://cdn.machinezoo.com/h/cKRPCJLDup30Q7xpWNcsaKZNp7y8am1zpP3PKZSutto/fvc2004-1b-png.zip";
				case "fvc2004-2b":
					return "https://cdn.machinezoo.com/h/6XCAX2TZjUZnm2bwBjvxhx4VSOk7D_-q7AO3yXfF9n8/fvc2004-2b-png.zip";
				case "fvc2004-3b":
					return "https://cdn.machinezoo.com/h/5A_W-WTx6R268rRmJJGhknGMbJcB8ik2RUk436e9_BA/fvc2004-3b-png.zip";
				case "fvc2004-4b":
					return "https://cdn.machinezoo.com/h/pP7exv5puFbdtkUNXmUucm9TgSbj94-0dEX_Fcj-jkQ/fvc2004-4b-png.zip";
				default:
					throw new ArgumentException();
			}
		}
		public static string Location(string dataset) { return Path.Combine(PersistentCache.Home, "samples", dataset); }
		static int Reported;
		public static string Unpack(string dataset)
		{
			var url = Url(dataset);
			var directory = Location(dataset);
			if (!Directory.Exists(directory))
			{
				var temporary = Path.Combine(Path.GetDirectoryName(directory), "tmp");
				if (Directory.Exists(temporary))
					Directory.Delete(temporary, true);
				Directory.CreateDirectory(temporary);
				if (Interlocked.Exchange(ref Reported, 1) == 0)
					Log.Information("Downloading sample fingerprints...");
				var download = temporary + ".zip";
				using (var client = new WebClient())
					client.DownloadFile(url, download);
				ZipFile.ExtractToDirectory(download, temporary);
				File.Delete(download);
				Directory.Move(temporary, directory);
			}
			return directory;
		}
	}
}
