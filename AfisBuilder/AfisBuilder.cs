using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;

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
            Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
            SolutionFolder = Directory.GetCurrentDirectory();
        }

        void UpdateVersions()
        {
            Versions.Collect();
            Versions.UpdateIn(@"SourceAFIS\Properties\AssemblyInfoMobile.cs");
            Versions.Update("SourceAFIS.Visualization");
            Versions.Update("SourceAFIS.Tuning");
            Versions.Update("SourceAFIS.Tests");
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
                Command.Build(@"SourceAFIS\SourceAFIS.Mobile.csproj", "Release Mobile");
                Command.Build(@"SourceAFIS.Visualization\SourceAFIS.Visualization.csproj", "Release");
                Command.Build(@"SourceAFIS.Tuning\SourceAFIS.Tuning.csproj", "Release");
                Command.Build(@"SourceAFIS.Tests\SourceAFIS.Tests.csproj", "Release");
                Command.Build(@"DatabaseAnalyzer\DatabaseAnalyzer.csproj", "Release");
                Command.Build(@"FingerprintAnalyzer\FingerprintAnalyzer.csproj", "Release");
                Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
                Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
            }
            else
                Command.BuildSolution("SourceAFIS.Mono.sln", "Release");
            Directory.CreateDirectory(Path.Combine("Sample", "dll"));
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", @"Sample\dll");
            Command.ForceDeleteDirectory(@"Sample\bin");
            if (!Mono)
                Command.Build(@"Sample\Sample.csproj", "Debug");
            else
                Command.BuildSolution(@"Sample\Sample.sln", "Debug");
            if (!Mono)
                Command.Build(@"DocProject\DocProject.csproj", "Release");
        }

        void AssembleZip()
        {
            ZipFolder = Path.Combine(OutputFolder, "SourceAFIS-" + Versions.Release);
            Console.WriteLine("Assembling ZIP archive: {0}", ZipFolder);
            Command.ForceDeleteDirectory(ZipFolder);
            Directory.CreateDirectory(ZipFolder);
            string prefix = Command.FixPath(ZipFolder + @"\");

            Directory.CreateDirectory(prefix + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", prefix + "Bin");
            if (!Mono)
            {
                Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", prefix + "Bin");
                Command.CopyTo(@"SourceAFIS\bin\Release Mobile\SourceAFIS.Mobile.dll", prefix + "Bin");
            }
            Command.CopyTo(@"SourceAFIS.Tuning\bin\Release\SourceAFIS.Visualization.dll", prefix + "Bin");
            Command.CopyTo(@"SourceAFIS.Tuning\bin\Release\SourceAFIS.Tuning.dll", prefix + "Bin");
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
            string workspace = Command.FixPath(OutputFolder + @"\msi");
            Console.WriteLine("Assembling MSI package: {0}", workspace);
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
            File.Copy(wxsPath, Path.Combine(workspace, wxsVersioned));
            Directory.SetCurrentDirectory(workspace);
            Command.CompileWiX(wxsVersioned);
            Directory.SetCurrentDirectory(SolutionFolder);
        }

        void AssembleFvcSubmission()
        {
            string fvcFolder = Path.Combine(OutputFolder, "SourceAFIS-FVC-" + Versions.Release);
            Console.WriteLine("Assembling FVC-onGoing submission: {0}", ZipFolder);
            Command.ForceDeleteDirectory(fvcFolder);
            Directory.CreateDirectory(fvcFolder);

            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", fvcFolder);
            Command.CopyTo(@"FvcEnroll\bin\Release\enroll.exe", fvcFolder);
            Command.CopyTo(@"FvcMatch\bin\Release\match.exe", fvcFolder);

            Command.ZipFiles(fvcFolder, new[] { "SourceAFIS.dll", "enroll.exe", "match.exe" });
        }

        void RunNUnitTests()
        {
            Command.Execute(
                @"C:\Program Files\NUnit 2.5.7\bin\net-2.0\nunit-console.exe",
                @"/xml=SourceAFIS.Tests\bin\Release\TestResult.xml",
                "/labels", "/nodots",
                @"SourceAFIS.Tests\bin\Release\SourceAFIS.Tests.dll");
        }

        void RunAnalyzer()
        {
            string analyzerDir = Path.Combine(OutputFolder, "Analyzer");
            Command.ForceDeleteDirectory(analyzerDir);
            Directory.CreateDirectory(analyzerDir);
            Directory.SetCurrentDirectory(analyzerDir);

            Analyzer.DatabasePath = Path.Combine("..", "..", "..", "..", "Data", "TestDatabase");
            Analyzer.PrepareXmlConfiguration(
                Path.Combine(SolutionFolder, "Data", "DatabaseAnalyzerConfiguration.xml"),
                "DatabaseAnalyzerConfiguration.xml");

            Command.CopyTo(ZipFolder + @"\Bin\DatabaseAnalyzer.exe", analyzerDir);
            Command.CopyTo(ZipFolder + @"\Bin\SourceAFIS.dll", analyzerDir);
            Command.CopyTo(ZipFolder + @"\Bin\SourceAFIS.Visualization.dll", analyzerDir);
            Command.CopyTo(ZipFolder + @"\Bin\SourceAFIS.Tuning.dll", analyzerDir);
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
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            SetFolder();
            UpdateVersions();
            BuildProjects();
            AssembleZip();
            if (!Mono)
            {
                AssembleMsi();
                AssembleFvcSubmission();
            }
            if (!Mono)
                RunNUnitTests();
            RunAnalyzer();
            Summary();
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
