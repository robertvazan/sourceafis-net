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
    }
}
