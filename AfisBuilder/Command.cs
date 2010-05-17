using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace AfisBuilder
{
    class Command
    {
        public static void ChangeDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        public static void Copy(string from, string to)
        {
            File.Copy(from, to, true);
        }

        public static void Build(string project, string configuration)
        {
            string[] versions = Directory.GetDirectories(@"C:\WINDOWS\Microsoft.NET\Framework", "v2.0.*");
            if (versions.Length == 0)
                throw new ApplicationException("Cannot find msbuild tool.");
            string msbuildPath = versions[versions.Length - 1] + @"\msbuild.exe";
            Execute(msbuildPath, "/t:Rebuild", "/p:configuration=" + configuration, project);
        }

        public static void Execute(string command, params string[] parameters)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = String.Join(" ", parameters);
            Console.WriteLine("Running: {0} {1}", command, process.StartInfo.Arguments);
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new ApplicationException("Child process failed.");
        }
    }
}
