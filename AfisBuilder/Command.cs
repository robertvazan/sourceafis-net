using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace AfisBuilder
{
    class Command
    {
        public static void CopyTo(string file, string folder)
        {
            File.Copy(file, folder + @"\" + Path.GetFileName(file), true);
        }

        public static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (string filename in Directory.GetFiles(from))
                CopyTo(filename, to);
            foreach (string subfolder in Directory.GetDirectories(from))
                if (Path.GetFileName(subfolder) != ".svn")
                    CopyDirectory(subfolder, to + @"\" + Path.GetFileName(subfolder));
        }

        public static void ForceDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void Build(string project, string configuration)
        {
            string[] versions = Directory.GetDirectories(@"C:\WINDOWS\Microsoft.NET\Framework", "v2.0.*");
            if (versions.Length == 0)
                throw new ApplicationException("Cannot find msbuild tool.");
            string msbuildPath = versions[versions.Length - 1] + @"\msbuild.exe";
            Execute(msbuildPath, "/t:Rebuild", "/p:configuration=" + configuration, project);
        }

        public static void Zip(string archive, string folder)
        {
            Execute(@"C:\Program Files\7-Zip\7z.exe", "a", "-tzip", archive, folder);
            if (!File.Exists(archive))
                throw new ApplicationException("No ZIP file was created.");
        }

        public static void CompileWiX(string project)
        {
            string bin = @"C:\Program Files\Windows Installer XML v3\bin\";
            Execute(bin + "candle.exe", project);
            Execute(bin + "light.exe", Path.GetFileNameWithoutExtension(project) + ".wixobj");
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
