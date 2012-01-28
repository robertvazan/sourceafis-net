using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SourceAFIS.Tests.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            NUnit.ConsoleRunner.Runner.Main(new string[]
            {
                typeof(SourceAFIS.Tests.Executable.Installer).Assembly.Location
            });
        }
    }
}
