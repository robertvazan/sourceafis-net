using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Linq;

namespace AfisBuilder
{
    class AfisBuilder
    {
        bool Mono = Type.GetType("Mono.Runtime") != null;

        string OutputFolder;
        string SolutionFolder;
        string ZipFolder;
        string MsiFolder;

        void SetFolder()
        {
            OutputFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
            SolutionFolder = Directory.GetCurrentDirectory();
        }

        void UpdateVersions()
        {
            Versions.Collect();
            Versions.UpdateIn(Path.Combine("SourceAFIS", "Properties", "AssemblyInfoMobile.cs"));
            Versions.Update("SourceAFIS.Visualization");
            Versions.Update("SourceAFIS.Tuning");
            Versions.Update("SourceAFIS.Tests");
            Versions.Update("DatabaseAnalyzer");
            Versions.Update("SourceAFIS.FingerprintAnalysis");
            if (!Mono)
            {
                Versions.Update("FvcEnroll");
                Versions.Update("FvcMatch");
                Versions.Update("FvcIso");
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
                Command.Build(@"SourceAFIS.FingerprintAnalysis\SourceAFIS.FingerprintAnalysis.csproj", "Release");
                Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
                Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
                Command.Build(@"FvcIso\FvcIso.csproj", "Release");
            }
            else
                Command.BuildSolution("SourceAFIS.Mono.sln", "Release");
            Directory.CreateDirectory(Path.Combine("Sample", "dll"));
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", @"Sample\dll");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", @"Sample\dll");
            if (!Mono)
			{
	            Command.ForceDeleteDirectory(@"Sample\bin");
                Command.Build(@"Sample\Sample.csproj", "Debug");
                Command.Build(@"DocProject\DocProject.csproj", "Release");
                Command.BuildAnt("sourceafis", "clean", "jar");
			}
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
	            Command.CopyTo(@"SourceAFIS.Visualization\bin\Release\SourceAFIS.Visualization.dll", prefix + "Bin");
	            Command.CopyTo(@"SourceAFIS.FingerprintAnalysis\bin\Release\SourceAFIS.FingerprintAnalysis.exe", prefix + "Bin");
            }
            Command.CopyTo(@"SourceAFIS.Tuning\bin\Release\SourceAFIS.Tuning.dll", prefix + "Bin");
            Command.CopyTo(@"DatabaseAnalyzer\bin\Release\DatabaseAnalyzer.exe", prefix + "Bin");
            Command.CopyTo(@"DatabaseAnalyzer\bin\Release\DatabaseAnalyzer.exe.config", prefix + "Bin");
            Command.CopyTo(@"Data\DatabaseAnalyzerConfiguration.xml", prefix + "Bin");
            if (!Mono)
                Command.CopyTo(@"java\sourceafis\bin\dist\sourceafis.jar", prefix + "Bin");

            Directory.CreateDirectory(prefix + "Documentation");
            Command.CopyTo(@"Data\license.txt", prefix + "Documentation");
            Command.CopyTo(@"Data\SourceAFIS_Home.html", prefix + "Documentation");
            if (!Mono)
            {
                Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", prefix + "Documentation");
                Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.chm", prefix + "Documentation");
                Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.HxS", prefix + "Documentation");

				Directory.CreateDirectory(prefix + @"Documentation\SourceAFIS");
                Command.CopyDirectory(@"DocProject\Help\html", prefix + @"Documentation\SourceAFIS\html");
                Command.CopyDirectory(@"DocProject\Help\Icons", prefix + @"Documentation\SourceAFIS\icons");
                Command.CopyDirectory(@"DocProject\Help\Scripts", prefix + @"Documentation\SourceAFIS\scripts");
                Directory.CreateDirectory(prefix + @"Documentation\SourceAFIS\styles");
                File.Copy(@"DocProject\Help\Styles\Presentation.css", prefix + @"Documentation\SourceAFIS\styles\presentation.css");
                Command.CopyTo(@"DocProject\Help\Styles\TopicDesigner.css", prefix + @"Documentation\SourceAFIS\styles");

	            Command.CopyDirectory("Sample", prefix + "Sample");
	            Command.DeleteFileIfExists(prefix + @"Sample\Sample.suo");
	            Command.ForceDeleteDirectory(prefix + @"Sample\obj");
	            Command.ForceDeleteDirectory(prefix + @"Sample\bin\Release");
	            Command.DeleteFileIfExists(prefix + @"Sample\bin\Debug\Sample.exe.mdb");
            }

            Command.Zip(ZipFolder);
        }

        void AssembleMsi()
        {
            MsiFolder = Path.Combine(OutputFolder, "msi");
            Console.WriteLine("Assembling MSI package: {0}", MsiFolder);
            Command.ForceDeleteDirectory(MsiFolder);
            Command.CopyDirectory(ZipFolder, MsiFolder);

            string wxsPath = @"AfisBuilder\SourceAFIS.wxs";
            WiX.Load(wxsPath);
            WiX.UpdateVersion(Versions.Release);
            WiX.ScanFolders(MsiFolder);
            WiX.ScanFiles();
            WiX.RemoveOldFiles();
            WiX.RemoveOldFolders();
            WiX.AddMissingFolders();
            WiX.AddMissingFiles();
            WiX.Save(wxsPath);

            string wxsVersioned = "SourceAFIS-" + Versions.Release + ".wxs";
            File.Copy(wxsPath, Path.Combine(MsiFolder, wxsVersioned));
            Directory.SetCurrentDirectory(MsiFolder);
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
            Command.CopyTo(@"FvcEnroll\bin\Release\enroll.exe.config", fvcFolder);
            Command.CopyTo(@"FvcMatch\bin\Release\match.exe", fvcFolder);
            Command.CopyTo(@"FvcMatch\bin\Release\match.exe.config", fvcFolder);

            Command.ZipFiles(fvcFolder, new[] { "SourceAFIS.dll", "enroll.exe", "enroll.exe.config", "match.exe", "match.exe.config" });
        }

        void AssembleFvcIsoSubmission()
        {
            string fvcFolder = Path.Combine(OutputFolder, "SourceAFIS-FVCISO-" + Versions.Release);
            Console.WriteLine("Assembling FVC-onGoing ISO submission: {0}", ZipFolder);
            Command.ForceDeleteDirectory(fvcFolder);
            Directory.CreateDirectory(fvcFolder);

            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", fvcFolder);
            Command.CopyTo(@"FvcIso\bin\Release\match.exe", fvcFolder);
            Command.CopyTo(@"FvcIso\bin\Release\match.exe.config", fvcFolder);

            Command.ZipFiles(fvcFolder, new[] { "SourceAFIS.dll", "match.exe", "match.exe.config" });
        }

        void RunTests()
        {
            string folder = @"SourceAFIS.Tests\bin\Release";
            Command.CopyTo(Path.Combine(MsiFolder, "SourceAFIS-" + Versions.Release + ".msi"), folder);

            string analysisFolder = Path.Combine(folder, "FingerprintAnalysis");
            Command.ForceDeleteDirectory(analysisFolder);
            Directory.CreateDirectory(analysisFolder);
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", analysisFolder);
            Command.CopyTo(@"SourceAFIS.Visualization\bin\Release\SourceAFIS.Visualization.dll", analysisFolder);
            Command.CopyTo(@"SourceAFIS.FingerprintAnalysis\bin\Release\SourceAFIS.FingerprintAnalysis.exe", analysisFolder);

            Directory.SetCurrentDirectory(folder);

            string nunit = (from nunitRoot in Directory.GetDirectories(@"C:\Program Files", "NUnit *.*.*")
                            orderby nunitRoot
                            select Path.Combine(nunitRoot, "bin", "net-2.0", "nunit-console.exe")).Last();

            Command.Execute(nunit, "/labels", "/nodots", "/exclude=Special,Installer,UI,JavaData", "SourceAFIS.Tests.dll");
            Command.Execute(nunit, "/labels", "/nodots", "/include=UI", "SourceAFIS.Tests.dll");

            Command.Execute(nunit, "/labels", "/nodots", "/run=SourceAFIS.Tests.Executable.JavaData.Templates", "SourceAFIS.Tests.dll");
            Command.Execute(nunit, "/labels", "/nodots", "/include=JavaData", "SourceAFIS.Tests.dll");

            Command.Execute(nunit, "/labels", "/nodots", "/run=SourceAFIS.Tests.Executable.InstallerRun.Install", "SourceAFIS.Tests.dll");
            Command.Execute(nunit, "/labels", "/nodots", "/run=SourceAFIS.Tests.Executable.Installer", "SourceAFIS.Tests.dll");
            Command.Execute(nunit, "/labels", "/nodots", "/run=SourceAFIS.Tests.Executable.InstallerRun.Uninstall", "SourceAFIS.Tests.dll");

            Directory.SetCurrentDirectory(SolutionFolder);

            Command.BuildAnt("sourceafis", "test");
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
			if (!Mono)
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

        void Cleanup()
        {
            if (!Mono)
                Command.BuildAnt("sourceafis", "clean");
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
                AssembleFvcIsoSubmission();
            }
            if (!Mono)
                RunTests();
            RunAnalyzer();
            Summary();
            Cleanup();
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
