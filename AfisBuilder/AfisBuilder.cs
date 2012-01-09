using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AfisBuilder
{
    class AfisBuilder
    {
        bool Mono = Type.GetType("Mono.Runtime") != null;

        string OutputFolder;
        string SolutionFolder;
        string ZipFolder;

        void SetFolder()
        {
            OutputFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Command.FixPath(@"..\..\.."));
            SolutionFolder = Directory.GetCurrentDirectory();
        }

        void UpdateVersions()
        {
            Versions.Collect();
            Versions.Update("DatabaseAnalyzer");
            Versions.Update("FingerprintAnalyzer");
            if (!Mono)
            {
                Versions.Update("FvcEnroll");
                Versions.Update("FvcMatch");
            }
            Versions.Update("Sample");
        }

        void BuildProjects()
        {
            if (!Mono)
            {
                Command.Build(@"SourceAFIS\SourceAFIS.csproj", "Release");
                Command.Build(@"DatabaseAnalyzer\DatabaseAnalyzer.csproj", "Release");
                Command.Build(@"FingerprintAnalyzer\FingerprintAnalyzer.csproj", "Release");
                Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
                Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
            }
            else
                Command.BuildSolution("SourceAFIS.Mono.sln", "Release");
            Directory.CreateDirectory(Command.FixPath(@"Sample\dll"));
            Command.CopyTo(Command.FixPath(@"SourceAFIS\bin\Release\SourceAFIS.dll"), @"Sample\dll");
            Command.ForceDeleteDirectory(Command.FixPath(@"Sample\bin"));
            if (!Mono)
                Command.Build(@"Sample\Sample.csproj", "Debug");
            else
                Command.BuildSolution(@"Sample\Sample.sln", "Debug");
            if (!Mono)
                Command.Build(@"DocProject\DocProject.csproj", "Release");
        }

        void AssembleZip()
        {
            ZipFolder = Command.FixPath(OutputFolder + @"\SourceAFIS-" + Versions.Release);
            Console.WriteLine("Assembling ZIP archive: {0}", ZipFolder);
            Command.ForceDeleteDirectory(ZipFolder);
            Directory.CreateDirectory(ZipFolder);
            string prefix = Command.FixPath(ZipFolder + @"\");

            Directory.CreateDirectory(prefix + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", prefix + "Bin");
            if (!Mono)
                Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", prefix + "Bin");
            Command.CopyTo(@"DatabaseAnalyzer\bin\Release\DatabaseAnalyzer.exe", prefix + "Bin");
            Command.CopyTo(@"Data\DatabaseAnalyzerConfiguration.xml", prefix + "Bin");
            Command.CopyTo(@"FingerprintAnalyzer\bin\Release\FingerprintAnalyzer.exe", prefix + "Bin");

            Directory.CreateDirectory(prefix + "Documentation");
            Command.CopyTo(@"Data\license.txt", prefix + "Documentation");
            Command.CopyTo(@"Data\SourceAFIS_Home.html", prefix + "Documentation");
            if (!Mono)
            {
                Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", prefix + "Documentation");
                Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.chm", prefix + "Documentation");
                Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.HxS", prefix + "Documentation");
            }

            if (!Mono)
            {
                Directory.CreateDirectory(prefix + @"Documentation\SourceAFIS");
                Command.CopyDirectory(@"DocProject\Help\html", prefix + @"Documentation\SourceAFIS\html");
                Command.CopyDirectory(@"DocProject\Help\Icons", prefix + @"Documentation\SourceAFIS\icons");
                Command.CopyDirectory(@"DocProject\Help\Scripts", prefix + @"Documentation\SourceAFIS\scripts");
                Directory.CreateDirectory(prefix + @"Documentation\SourceAFIS\styles");
                File.Copy(@"DocProject\Help\Styles\Presentation.css", prefix + @"Documentation\SourceAFIS\styles\presentation.css");
                Command.CopyTo(@"DocProject\Help\Styles\TopicDesigner.css", prefix + @"Documentation\SourceAFIS\styles");
            }

            Command.CopyDirectory("Sample", prefix + "Sample");
            Command.DeleteFileIfExists(prefix + @"Sample\Sample.suo");
            Command.ForceDeleteDirectory(prefix + @"Sample\obj");
            Command.DeleteFileIfExists(prefix + @"Sample\bin\Debug\Sample.exe.mdb");

            Command.Zip(ZipFolder);
        }

        void AssembleMsi()
        {
            string workspace = OutputFolder + @"\msi";
            Command.ForceDeleteDirectory(workspace);
            Command.CopyDirectory(ZipFolder, workspace);

            string wxsPath = @"AfisBuilder\SourceAFIS.wxs";
            WiX.Load(wxsPath);
            WiX.UpdateVersion(Versions.Release);
            WiX.ScanFolders(workspace);
            WiX.ScanFiles();
            WiX.RemoveOldFiles();
            WiX.RemoveOldFolders();
            WiX.AddMissingFolders();
            WiX.AddMissingFiles();
            WiX.Save(wxsPath);

            string wxsVersioned = "SourceAFIS-" + Versions.Release + ".wxs";
            File.Copy(wxsPath, workspace + @"\" + wxsVersioned);
            Directory.SetCurrentDirectory(workspace);
            Command.CompileWiX(wxsVersioned);
            Directory.SetCurrentDirectory(SolutionFolder);
        }

        void RunAnalyzer()
        {
            string analyzerDir = Path.Combine(OutputFolder, "Analyzer");
            Command.ForceDeleteDirectory(analyzerDir);
            Directory.CreateDirectory(analyzerDir);
            Directory.SetCurrentDirectory(analyzerDir);

            Analyzer.DatabasePath = Command.FixPath(@"..\..\..\..\Data\TestDatabase");
            Analyzer.PrepareXmlConfiguration(
                Command.FixPath(SolutionFolder + @"\Data\DatabaseAnalyzerConfiguration.xml"),
                "DatabaseAnalyzerConfiguration.xml");

            Command.CopyTo(ZipFolder + @"\Bin\DatabaseAnalyzer.exe", analyzerDir);
            Command.CopyTo(ZipFolder + @"\Bin\SourceAFIS.dll", analyzerDir);
            Command.Execute(Path.Combine(analyzerDir, "DatabaseAnalyzer.exe"));

            Analyzer.ReadAccuracy();
            Analyzer.ReadSpeed();
            Analyzer.ReadExtractorStats();

            Directory.SetCurrentDirectory(SolutionFolder);
        }

        void Summary()
        {
            Analyzer.ReportStatistics();
            Console.WriteLine("AfisBuilder finished successfully.");
        }

        public void Run()
        {
            SetFolder();
            UpdateVersions();
            BuildProjects();
            AssembleZip();
            if (!Mono)
                AssembleMsi();
            RunAnalyzer();
            Summary();
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
