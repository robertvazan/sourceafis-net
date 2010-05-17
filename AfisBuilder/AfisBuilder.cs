using System;
using System.Collections.Generic;
using System.Text;

namespace AfisBuilder
{
    class AfisBuilder
    {
        public void Run()
        {
            Command.ChangeDirectory(@"..\..\..");
            Command.Build(@"SourceAFIS\SourceAFIS.csproj", "Release");
            Command.Build(@"DatabaseAnalyzer\DatabaseAnalyzer.csproj", "Release");
            Command.Build(@"FingerprintAnalyzer\FingerprintAnalyzer.csproj", "Release");
            Command.Build(@"FvcEnroll\FvcEnroll.csproj", "Release");
            Command.Build(@"FvcMatch\FvcMatch.csproj", "Release");
            Command.Copy(@"SourceAFIS\bin\Release\SourceAFIS.dll", @"Sample\dll\SourceAFIS.dll");
            Command.Build(@"Sample\Sample.csproj", "Debug");
            Command.Build(@"DocProject\DocProject.csproj", "Release");
        }

        static void Main(string[] args)
        {
            new AfisBuilder().Run();
        }
    }
}
