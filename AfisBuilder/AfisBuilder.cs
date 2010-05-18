using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AfisBuilder
{
    class AfisBuilder
    {
        string OutputFolder;

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
            string path = OutputFolder + @"\SourceAFIS-" + Versions.Release;
            Console.WriteLine("Assembling ZIP archive: {0}", path);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            path += @"\";

            Directory.CreateDirectory(path + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.dll", path + "Bin");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", path + "Bin");
            Command.CopyTo(@"DatabaseAnalyzer\bin\Release\DatabaseAnalyzer.exe", path + "Bin");
            Command.CopyTo(@"Data\DatabaseAnalyzerConfiguration.xml", path + "Bin");
            Command.CopyTo(@"FingerprintAnalyzer\bin\Release\FingerprintAnalyzer.exe", path + "Bin");

            Directory.CreateDirectory(path + "Documentation");
            Command.CopyTo(@"Data\license.txt", path + "Documentation");
            Command.CopyTo(@"Data\SourceAFIS_Home.html", path + "Documentation");
            Command.CopyTo(@"SourceAFIS\bin\Release\SourceAFIS.xml", path + "Documentation");
            Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.chm", path + "Documentation");
            Command.CopyTo(@"DocProject\bin\Release\SourceAFIS.HxS", path + "Documentation");

            Directory.CreateDirectory(path + @"Documentation\SourceAFIS");
            Command.CopyDirectory(@"DocProject\Help\html", path + @"Documentation\SourceAFIS\html");
            Command.CopyDirectory(@"DocProject\Help\Icons", path + @"Documentation\SourceAFIS\icons");
            Command.CopyDirectory(@"DocProject\Help\Scripts", path + @"Documentation\SourceAFIS\scripts");
            Directory.CreateDirectory(path + @"Documentation\SourceAFIS\styles");
            File.Copy(@"DocProject\Help\Styles\Presentation.css", path + @"Documentation\SourceAFIS\styles\presentation.css");
            Command.CopyTo(@"DocProject\Help\Styles\TopicDesigner.css", path + @"Documentation\SourceAFIS\styles");

            Command.CopyDirectory("Sample", path + "Sample");
            if (File.Exists(path + @"Sample\Sample.suo"))
                File.Delete(path + @"Sample\Sample.suo");
            Directory.Delete(path + @"Sample\obj", true);

            Command.Zip(OutputFolder + @"\SourceAFIS-" + Versions.Release + ".zip", path);
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
