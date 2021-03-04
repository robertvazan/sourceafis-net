// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using Serilog;
using SourceAFIS;

namespace SourceAFIS.Cmd
{
	static class SampleDownload
	{
		static readonly string[] Available = new[]
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
			switch (dataset) {
				case "fvc2000-1b":
					return "https://cdn.machinezoo.com/h/O_mBtWH-PXJ4ETJJe_G-Z9EmJoJLfq4srVw23tTEMZw/fvc2000-1b.zip";
				case "fvc2000-2b":
					return "https://cdn.machinezoo.com/h/zJB3za1cEccZjZmkV6KfD5Jk_ffegOmOcTZmG4PpaSM/fvc2000-2b.zip";
				case "fvc2000-3b":
					return "https://cdn.machinezoo.com/h/oGd8JtGpIzDSprQSsGNpbJuAAjNLTZxc_1Rol6t5deA/fvc2000-3b.zip";
				case "fvc2000-4b":
					return "https://cdn.machinezoo.com/h/624mM3sTCV8kZy75UilOMkEl-RFjv_9lGXIr9I7dzH8/fvc2000-4b.zip";
				case "fvc2002-1b":
					return "https://cdn.machinezoo.com/h/ZGusAOeUs8zVmtCtFdUbNCyAqV2qFEtaFw2GWxyrRFo/fvc2002-1b.zip";
				case "fvc2002-2b":
					return "https://cdn.machinezoo.com/h/N3FvC0y0dt684GsQkSrKynyj6PUYswCV7ak2xjPZFGI/fvc2002-2b.zip";
				case "fvc2002-3b":
					return "https://cdn.machinezoo.com/h/46O3Whe353EeJn8aIPCo0zWnddd5fSXsvVXSKTQCrOA/fvc2002-3b.zip";
				case "fvc2002-4b":
					return "https://cdn.machinezoo.com/h/GSLM0-GZULWBL2Dc6Lk6QuTs_FcwZgGHi6NiJrZupNc/fvc2002-4b.zip";
				case "fvc2004-1b":
					return "https://cdn.machinezoo.com/h/Owa1eWSvirTpEQ4NQfdJzKxNsBPfwJftpJjLkaVnoiw/fvc2004-1b.zip";
				case "fvc2004-2b":
					return "https://cdn.machinezoo.com/h/S7yLI6vOiFvog-PniaOCSdQ4etoNxGAEH81MfHvl_C8/fvc2004-2b.zip";
				case "fvc2004-3b":
					return "https://cdn.machinezoo.com/h/0zZbQizCzt2eVPE-QdKEz3VaDiKERGc1aFFGPouAirE/fvc2004-3b.zip";
				case "fvc2004-4b":
					return "https://cdn.machinezoo.com/h/nAFmSXlgm-bbTflylBBn5dRe775haHKgmK1T5tVnHRw/fvc2004-4b.zip";
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
