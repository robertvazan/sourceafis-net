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
    public class Startup : Common
    {
        [Test]
        public void DefaultStartup()
        {
            StartApp();
            Assert.IsNotNull(Win);
            Assert.IsTrue(Win.DisplayState == DisplayState.Maximized);
        }

        [TearDown]
        public void TearDown()
        {
            if (App != null && !App.HasExited)
                App.Kill();
        }
    }
}
