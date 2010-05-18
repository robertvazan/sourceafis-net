using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AfisBuilder
{
    class AfisBuilder
    {
        string OutputFolder;
        string ZipFolder;

        void SetFolder()
        {
            OutputFolder = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(@"..\..\..");
        }

        void UpdateVersions()
        {
            Versions.Collect();
            Versions.Update("DatabaseAnalyzer");
            Versions.Update("FingerprintAnalyzer");
            Versions.Update("FvcEnroll");
            Versions.Update("FvcMatch");
            Versions.Update("Sample");
        }

        void BuildProjects()
        {
            Command.Build(@"SourceAFIS\SourceAFIS.csproj", "Release");
            Command.Build(@"DatabaseAnalyzer\DatabaseAnalyzer.csproj", "Release");
            Command.Build(@"FingerprintAnalyzer\FingerprintAnalyzer.csproj", "Release");
            Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
            Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", @"Sample\dll");
            if (Directory.Exists(@"Sample\bin"))
                Directory.Delete(@"Sample\bin", true);
            Command.Build(@"Sample\Sample.csproj", "Debug");
            Command.Build(@"DocProject\DocProject.csproj", "Release");
        }

        void AssembleZip()
        {
            ZipFolder = OutputFolder + @"\SourceAFIS-" + Versions.Release;
            Console.WriteLine("Assembling ZIP archive: {0}", ZipFolder);
            if (Directory.Exists(ZipFolder))
                Directory.Delete(ZipFolder, true);
            Directory.CreateDirectory(ZipFolder);
            string prefix = ZipFolder + @"\";

            Directory.CreateDirectory(prefix + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", prefix + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", prefix + "Bin");
            Command.CopyTo(@"DatabaseAnalyzer\bin\Release\DatabaseAnalyzer.exe", prefix + "Bin");
            Command.CopyTo(@"Data\DatabaseAnalyzerConfiguration.xml", prefix + "Bin");
            Command.CopyTo(@"FingerprintAnalyzer\bin\Release\FingerprintAnalyzer.exe", prefix + "Bin");

            Directory.CreateDirectory(prefix + "Documentation");
            Command.CopyTo(@"Data\license.txt", prefix + "Documentation");
            Command.CopyTo(@"Data\SourceAFIS_Home.html", prefix + "Documentation");
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
            if (File.Exists(prefix + @"Sample\Sample.suo"))
                File.Delete(prefix + @"Sample\Sample.suo");
            Directory.Delete(prefix + @"Sample\obj", true);

            Command.Zip(ZipFolder + ".zip", ZipFolder);
        }

        public void Run()
        {
            SetFolder();
            UpdateVersions();
            BuildProjects();
            AssembleZip();
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
