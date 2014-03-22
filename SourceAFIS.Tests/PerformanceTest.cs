using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace SourceAFIS.Tests
{
    [TestFixture]
    public class PerformanceTest
    {
        [Test]
        public void Extract()
        {
            var db = Path.Combine("FVC2000", "DB1_B");
            var images = Path.Combine(TestUtils.ImagePath, db);
            new FingerprintTemplate(TestUtils.LoadImage(Path.Combine(images, "101_1.tif")));
            return;
            var templates = Path.Combine(TestUtils.TemplatePath, db);
            Directory.CreateDirectory(templates);
            foreach (var file in Directory.GetFiles(images))
                if (TestUtils.ImageExtensions.Contains(Path.GetExtension(file)))
                    new FingerprintTemplate(TestUtils.LoadImage(file)).ToXml()
                        .Save(Path.Combine(templates, Path.GetFileNameWithoutExtension(file) + ".xml"));
        }
    }
}
