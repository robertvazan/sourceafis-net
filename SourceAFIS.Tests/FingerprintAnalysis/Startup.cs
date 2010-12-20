using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems.WindowItems;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture]
    public class Startup
    {
        string AppPath = Path.Combine("FingerprintAnalysis", "SourceAFIS.FingerprintAnalysis.exe");
        Application App;

        [Test]
        public void DefaultStartup()
        {
            App = Application.Launch(AppPath);
            Window window = App.GetWindow("Fingerprint Analysis", InitializeOption.NoCache);
            Assert.IsNotNull(window);
            Assert.IsTrue(window.DisplayState == DisplayState.Maximized);
            App.Kill();
        }

        [TearDown]
        public void TearDown()
        {
            if (App != null && !App.HasExited)
                App.Kill();
        }
    }
}
