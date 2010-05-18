using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace AfisBuilder
{
    class Versions
    {
        static Regex Regex;
        public static string Release;

        public static void Collect()
        {
            Console.WriteLine("Reading release version");
            Regex = new Regex(@"^\[assembly: AssemblyVersion\(""(\d+.\d+).\*""\)\]$");
            foreach (string line in File.ReadAllLines(@"SourceAFIS\Properties\AssemblyInfo.cs"))
            {
                Match match = Regex.Match(line);
                if (match.Success)
                    Release = match.Groups[1].Value;
            }
            if (Release == null)
                throw new ApplicationException("Release version not found.");
        }

        public static void Update(string project)
        {
            string path = project + @"\Properties\AssemblyInfo.cs";
            Console.WriteLine("Updating version: {0}", path);
            string[] lines = File.ReadAllLines(path);
            bool found = false;
            for (int i = 0; i < lines.Length; ++i)
            {
                Match match = Regex.Match(lines[i]);
                if (match.Success)
                {
                    found = true;
                    Group group = match.Groups[1];
                    int from = group.Index;
                    int to = group.Index + group.Length;
                    lines[i] = lines[i].Substring(0, from) + Release + lines[i].Substring(to, lines[i].Length - to);
                }
            }
            if (!found)
                throw new ApplicationException("No version to update.");
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}
