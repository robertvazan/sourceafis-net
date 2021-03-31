// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
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
        public enum Format
        {
            Original,
            Png,
            Gray
        }
        public static readonly Format DefaultFormat = Format.Gray;
        static string Url(String dataset, Format format)
        {
            switch (format)
            {
                case Format.Original:
                    switch (dataset)
                    {
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
                case Format.Png:
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
                case Format.Gray:
                    switch (dataset)
                    {
                        case "fvc2000-1b":
                            return "https://cdn.machinezoo.com/h/AkBMOzR_T_0_UmxZXaubrYmwmcR1yOnByJvl3AWieMI/fvc2000-1b-gray.zip";
                        case "fvc2000-2b":
                            return "https://cdn.machinezoo.com/h/GBo_uNlW3166tHV-_QXTCWWo6YywNycOz_n4AUQhO3Y/fvc2000-2b-gray.zip";
                        case "fvc2000-3b":
                            return "https://cdn.machinezoo.com/h/6BXcjr6ZvCr4MrAYC5yiFioYCrepCiBfg68SrR0puxo/fvc2000-3b-gray.zip";
                        case "fvc2000-4b":
                            return "https://cdn.machinezoo.com/h/8lbaA4LGUeNFxbLbazAG-ji76_pQV3nJpCnlY__ncAc/fvc2000-4b-gray.zip";
                        case "fvc2002-1b":
                            return "https://cdn.machinezoo.com/h/kTJNA8M9KRnrsUPYiz4Pty5V1FPzFbdnemNqRRRsu90/fvc2002-1b-gray.zip";
                        case "fvc2002-2b":
                            return "https://cdn.machinezoo.com/h/7ghKDoqMr2C-OFwuqRWy-1rmdYNM3f-Zu-dy4g8SN6c/fvc2002-2b-gray.zip";
                        case "fvc2002-3b":
                            return "https://cdn.machinezoo.com/h/JTyQDvcQFE-WTeOKk8QuPAalDWvVV6SgVXIH1gNKQ8s/fvc2002-3b-gray.zip";
                        case "fvc2002-4b":
                            return "https://cdn.machinezoo.com/h/TsMV_b91QIx-cgq-FfPRH7MdE8XYJzL6ovCNJyAgYoU/fvc2002-4b-gray.zip";
                        case "fvc2004-1b":
                            return "https://cdn.machinezoo.com/h/3z2urqUag2AQT7m0cLmT14ofkpd6TCGlGdfagbiSScU/fvc2004-1b-gray.zip";
                        case "fvc2004-2b":
                            return "https://cdn.machinezoo.com/h/pTR8G8tQgaYRQSz3Gip8_eDLlg4G3OgvGfqDuoNOHkQ/fvc2004-2b-gray.zip";
                        case "fvc2004-3b":
                            return "https://cdn.machinezoo.com/h/I_jWMHnQE2J7qi3YOJbh9FwU0ObiFYOdunHYKqJW8K0/fvc2004-3b-gray.zip";
                        case "fvc2004-4b":
                            return "https://cdn.machinezoo.com/h/elY4DqdhFK8kukU9ZHmV_H8JgL2xETg1Oz74Bg1On4s/fvc2004-4b-gray.zip";
                        default:
                            throw new ArgumentException();
                    }
                default:
                    throw new ArgumentException();
            }
        }
        public static string Location(string dataset, Format format)
        {
            string name;
            switch (format)
            {
                case Format.Original:
                    name = "samples";
                    break;
                case Format.Png:
                    name = "png-samples";
                    break;
                case Format.Gray:
                    name = "gray-samples";
                    break;
                default:
                    throw new ArgumentException();
            }
            return Path.Combine(PersistentCache.Home, name, dataset);
        }
        static int Reported;
        public static string Unpack(string dataset, Format format)
        {
            var directory = Location(dataset, format);
            if (!Directory.Exists(directory))
            {
                var url = Url(dataset, format);
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
