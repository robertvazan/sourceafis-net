using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.Finders;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture, RequiresSTA]
    [Category("UI")]
    public class Options : Common
    {
        [Test]
        public void ClickThrough()
        {
            SelectFiles();
            ResetOptions();
            FullOptions();
        }

        [Test]
        public void NotFullyLoaded()
        {
            CloseFiles();
            FullOptions();
            SelectFile(Left, Settings.SomeFingerprintPath);
            SelectFile(Left, null);
            SelectFile(Right, Settings.MatchingFingerprintPath);
            SelectFile(Left, Settings.SomeFingerprintPath);
            SelectFile(Right, null);
            SelectFile(Left, null);
        }
    }
}
