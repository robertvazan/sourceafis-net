using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AfisBuilder
{
    class Command
    {
        public static bool Mono = Type.GetType("Mono.Runtime") != null;

        public static string FixPath(string original)
        {
            return original.Replace('\\', Path.DirectorySeparatorChar);
        }
        
        public static void CopyTo(string file, string folder)
        {
            File.Copy(FixPath(file), Path.Combine(FixPath(folder), Path.GetFileName(FixPath(file))), true);
        }

        public static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(FixPath(to));
            foreach (string filename in Directory.GetFiles(from))
                CopyTo(filename, to);
            foreach (string subfolder in Directory.GetDirectories(from))
                if (Path.GetFileName(subfolder) != ".svn")
                    CopyDirectory(subfolder, to + Path.DirectorySeparatorChar + Path.GetFileName(subfolder));
        }

        public static void ForceDeleteDirectory(string path)
        {
            path = FixPath(path);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void DeleteFileIfExists(string path)
        {
            path = FixPath(path);
            if (File.Exists(path))
                File.Delete(path);
        }
        
        public static void Build(string project, string configuration)
        {
            string[] versions = Directory.GetDirectories(@"C:\WINDOWS\Microsoft.NET\Framework", "v4.0.*");
            if (versions.Length == 0)
                throw new ApplicationException("Cannot find msbuild tool.");
            string msbuildPath = versions[versions.Length - 1] + @"\msbuild.exe";
            Execute(msbuildPath, "/t:Build", "\"/p:configuration=" + configuration + "\"", project);
        }

        public static void BuildSolution(string solution, string configuration)
        {
            Execute("mdtool", "build", "--configuration:" + configuration, FixPath(solution));
        }

        public static void BuildAnt(string project, params string[] targets)
        {
            string oldDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(@"java\" + project);
            string[] versions = Directory.GetDirectories(@"C:\Program Files", "apache-ant-*");
            if (versions.Length == 0)
                throw new ApplicationException("Cannot find ant.");
            string antPath = versions[versions.Length - 1] + @"\bin\ant.bat";
            Execute(antPath, targets);
            Directory.SetCurrentDirectory(oldDir);
        }

        public static void Zip(string contents)
        {
            contents = FixPath(contents);
            string folder = Path.GetDirectoryName(contents);
            contents = Path.GetFileName(contents);
            string archive = contents + ".zip";
            string oldFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(folder);
            DeleteFileIfExists(archive);
            if (!Mono)
                Execute(@"C:\Program Files\7-Zip\7z.exe", "a", "-tzip", archive, contents);
            else
                Execute("zip", "-r", archive, contents);
            if (!File.Exists(archive))
                throw new ApplicationException("No ZIP file was created.");
            Directory.SetCurrentDirectory(oldFolder);
        }

        public static void UnZip(string zip)
        {
            zip = FixPath(zip);
            string folder = Path.GetDirectoryName(zip);
            zip = Path.GetFileName(zip);
            string oldFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(folder);
            string unpacked = zip.Substring(0, zip.Length - 4);
            ForceDeleteDirectory(unpacked);
            if (!Mono)
                Execute(@"C:\Program Files\7-Zip\7z.exe", "x", zip);
            else
                Execute("unzip", zip);
            if (!Directory.Exists(unpacked))
                throw new ApplicationException("ZIP file couldn't be unpacked.");
            Directory.SetCurrentDirectory(oldFolder);
        }

        public static void ZipFiles(string folder, string[] contents)
        {
            folder = FixPath(folder);
            string archive = Path.GetFileName(folder) + ".zip";
            string oldFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(folder);
            DeleteFileIfExists(archive);
            Execute(@"C:\Program Files\7-Zip\7z.exe", new[] { "a", "-tzip", archive }.Concat(contents).ToArray());
            if (!File.Exists(archive))
                throw new ApplicationException("No ZIP file was created.");
            Directory.SetCurrentDirectory(oldFolder);
        }

        public static void CompileWiX(string project)
        {
            string bin = @"C:\Program Files\Windows Installer XML v3.5\bin\";
            Execute(bin + "candle.exe", "-ext", "WiXUtilExtension", project);
            Execute(bin + "light.exe", "-ext", "WiXNetFxExtension", "-ext", "WixUIExtension",
                "-ext", "WiXUtilExtension", Path.GetFileNameWithoutExtension(project) + ".wixobj");
            if (!File.Exists(Path.GetFileNameWithoutExtension(project) + ".msi"))
                throw new ApplicationException("No MSI file was created.");
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
