using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AfisBuilder
{
    class AfisBuilder
    {
        Regex VersionRegex;
        string BuildVersion;

        void CollectVersion()
        {
            Console.WriteLine("Reading release version");
            VersionRegex = new Regex(@"^\[assembly: AssemblyVersion\(""(\d+.\d+).\*""\)\]$");
            foreach (string line in File.ReadAllLines(@"SourceAFIS\Properties\AssemblyInfo.cs"))
            {
                Match match = VersionRegex.Match(line);
                if (match.Success)
                    BuildVersion = match.Groups[0].Value;
            }
            if (BuildVersion == null)
                throw new ApplicationException("Release version not found.");
        }

        void UpdateVersion(string project)
        {
            string path = project + @"\Properties\AssemblyInfo.cs";
            Console.WriteLine("Updating version: {0}", path);
            string[] lines = File.ReadAllLines(path);
            bool found = false;
            for (int i = 0; i < lines.Length; ++i)
            {
                Match match = VersionRegex.Match(lines[i]);
                if (match.Success)
                {
                    found = true;
                    Group group = match.Groups[0];
                    int from = group.Index;
                    int to = group.Index + group.Length;
                    lines[i] = lines[i].Substring(0, from) + BuildVersion + lines[i].Substring(to, lines[i].Length - to);
                }
            }
            if (!found)
                throw new ApplicationException("No version to update.");
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        public void Run()
        {
            Directory.SetCurrentDirectory(@"..\..\..");
            CollectVersion();
            UpdateVersion("DatabaseAnalyzer");
            UpdateVersion("FingerprintAnalyzer");
            UpdateVersion("FvcEnroll");
            UpdateVersion("FvcMatch");
            UpdateVersion("Sample");
            Command.Build(@"SourceAFIS\SourceAFIS.csproj", "Release");
            Command.Build(@"DatabaseAnalyzer\DatabaseAnalyzer.csproj", "Release");
            Command.Build(@"FingerprintAnalyzer\FingerprintAnalyzer.csproj", "Release");
            Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
            Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
            File.Copy(@"SourceAFIS\bin\Release\SourceAFIS.dll", @"Sample\dll\SourceAFIS.dll", true);
            Command.Build(@"Sample\Sample.csproj", "Debug");
            Command.Build(@"DocProject\DocProject.csproj", "Release");
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
