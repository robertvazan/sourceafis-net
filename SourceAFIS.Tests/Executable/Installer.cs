using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SourceAFIS.Tests.Executable
{
    [TestFixture]
    [Category("Executable")]
    public class Installer
    {
        [SetUp]
        public void SetUp()
        {
            string msi = Directory.GetFiles(Directory.GetCurrentDirectory(), "SourceAFIS-*.msi")[0];
            Process msiInstall = Process.Start("msiexec", "/q /i \"" + msi + "\"");
            msiInstall.WaitForExit();
            Assert.AreEqual(0, msiInstall.ExitCode);
        }

        [Test]
        public void Run()
        {
            string pfiles = @"C:\Program Files\SourceAFIS";
            Assert.That(Directory.Exists(pfiles));

            string bin = Path.Combine(pfiles, "Bin");
            Assert.That(Directory.Exists(bin));
            Assert.That(File.Exists(Path.Combine(bin, "SourceAFIS.dll")));
            Assert.That(File.Exists(Path.Combine(bin, "SourceAFIS.Visualization.dll")));
            Assert.That(File.Exists(Path.Combine(bin, "FingerprintAnalyzer.exe")));

            string sample = Path.Combine(pfiles, "Sample");
            Assert.That(Directory.Exists(sample));
            Assert.That(File.Exists(Path.Combine(sample, "Sample.sln")));
            Assert.AreEqual(4, Directory.GetFiles(Path.Combine(sample, "images")).Length);
            Assert.That(File.Exists(Path.Combine(sample, "dll", "SourceAFIS.dll")));
            Assert.That(File.Exists(Path.Combine(sample, "bin", "Debug", "Sample.exe")));

            string doc = Path.Combine(pfiles, "Documentation");
            Assert.That(Directory.Exists(doc));
            Assert.That(File.Exists(Path.Combine(doc, "SourceAFIS_Home.html")));
            Assert.That(File.Exists(Path.Combine(doc, "SourceAFIS.chm")));
            Assert.That(File.Exists(Path.Combine(doc, "SourceAFIS", "html", "N_SourceAFIS_Simple.htm")));
            Assert.That(File.Exists(Path.Combine(doc, "SourceAFIS", "icons", "pubclass.gif")));

            string start = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "SourceAFIS");
            Assert.That(Directory.Exists(start));
            Assert.That(File.Exists(Path.Combine(start, "Program Files.lnk")));
            Assert.That(File.Exists(Path.Combine(start, "Project Homepage.lnk")));
            Assert.That(File.Exists(Path.Combine(start, "Fingerprint Analyzer.lnk")));

            TearDown();

            Assert.That(!Directory.Exists(pfiles));
            Assert.That(!Directory.Exists(start));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(@"C:\Program Files\SourceAFIS"))
            {
                string msi = Directory.GetFiles(Directory.GetCurrentDirectory(), "SourceAFIS-*.msi")[0];
                Process msiInstall = Process.Start("msiexec", "/q /x \"" + msi + "\"");
                msiInstall.WaitForExit();
                Assert.AreEqual(0, msiInstall.ExitCode);
            }
        }
    }
}
