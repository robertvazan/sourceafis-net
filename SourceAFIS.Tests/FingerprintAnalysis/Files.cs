using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture]
    public class Files : Common
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            StartApp();
        }

        [Test]
        public void ClickThrough()
        {
            ResetOptions();
            Orientation.Checked = true;

            SelectFile(Left, null);
            SelectFile(Right, null);

            SelectFiles();

            SaveFile(Left);
            SaveFile(Right);

            SelectFile(Left, null);
            SelectFile(Right, null);
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(300);
            Assert.AreEqual(0, Win.ModalWindows().Count);
            Assert.IsFalse(App.HasExited);
        }
    }
}
