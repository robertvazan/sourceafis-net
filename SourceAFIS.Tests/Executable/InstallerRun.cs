using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace SourceAFIS.Tests.Executable
{
    [TestFixture]
    [Category("Special")]
    class InstallerRun
    {
        [Test]
        public void Install()
        {
            string msi = Directory.GetFiles(Directory.GetCurrentDirectory(), "SourceAFIS-*.msi")[0];
            Process msiInstall = Process.Start("msiexec", "/q /i \"" + msi + "\"");
            msiInstall.WaitForExit();
            Assert.AreEqual(0, msiInstall.ExitCode);
        }

        [Test]
        public void Uninstall()
        {
            string msi = Directory.GetFiles(Directory.GetCurrentDirectory(), "SourceAFIS-*.msi")[0];
            Process msiInstall = Process.Start("msiexec", "/q /x \"" + msi + "\"");
            msiInstall.WaitForExit();
            Assert.AreEqual(0, msiInstall.ExitCode);

            string pfiles = @"C:\Program Files\SourceAFIS";
            string start = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "SourceAFIS");
            Assert.That(!Directory.Exists(pfiles));
            Assert.That(!Directory.Exists(start));
        }
    }
}
