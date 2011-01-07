using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems.WindowItems;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture, RequiresSTA]
    public class Startup : Common
    {
        public override void TestFixtureSetUp()
        {
        }

        public override void TearDown()
        {
            if (App != null && !App.HasExited)
            {
                base.TearDown();
                base.TestFixtureTearDown();
            }
        }

        [Test]
        public void DefaultStartup()
        {
            StartApp();
            Assert.IsNotNull(Win);
            Assert.IsTrue(Win.DisplayState == DisplayState.Maximized);
        }

        [Test]
        public void SavedOptions()
        {
            StartApp();
            CloseFiles();
            ResetOptions();
            CloseApp();

            StartApp();
            Assert.AreEqual("", Left.FileName.Text);
            Assert.AreEqual("", Right.FileName.Text);
            Assert.AreEqual(GetResetOptions(), GetOptions());

            SelectFiles();
            FullOptions();
            CloseApp();

            StartApp();
            Assert.AreEqual(Path.GetFileNameWithoutExtension(Settings.SomeFingerprintPath), Left.FileName.Text);
            Assert.AreEqual(Path.GetFileNameWithoutExtension(Settings.MatchingFingerprintPath), Right.FileName.Text);
            Assert.AreEqual(GetFullOptions(), GetOptions());
        }

        [Test]
        public void VersionChange()
        {
            StartApp();
            SelectFiles();
            FullOptions();
            CloseApp();

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SourceAFIS", "FingerprintAnalysisSettings.xml");
            XDocument xml = XDocument.Load(path);
            xml.Root.Attribute("ProgramVersion").SetValue("invalid");
            xml.Save(path);

            StartApp();
            Assert.AreEqual("", Left.FileName.Text);
            Assert.AreEqual("", Right.FileName.Text);
            Assert.AreEqual(GetResetOptions(), GetOptions());

            CloseApp();
            xml = XDocument.Load(path);
            Assert.AreNotEqual("invalid", (string)xml.Root.Attribute("ProgramVersion"));
        }
    }
}
