using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AfisBuilder
{
    class AfisBuilder
    {
        public void Run()
        {
            Directory.SetCurrentDirectory(@"..\..\..");
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
