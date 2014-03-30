using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Xml.Linq;

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
            var templates = Path.Combine(TestUtils.TemplatePath, db);
            Directory.CreateDirectory(templates);
            foreach (var file in Directory.GetFiles(images))
                if (TestUtils.ImageExtensions.Contains(Path.GetExtension(file)))
                    new FingerprintTemplate(TestUtils.LoadImage(file)).ToXml()
                        .Save(Path.Combine(templates, Path.GetFileNameWithoutExtension(file) + ".xml"));
        }

		[Test]
		public void Match()
		{
			var db = Path.Combine("FVC2000", "DB1_B");
			Console.WriteLine("Matching database {0}", db);
			var templates = Path.Combine(TestUtils.TemplatePath, db);
			var personIds = (from filename in Directory.GetFiles(templates).Take(5)
			                 select Path.GetFileNameWithoutExtension(filename).Split(new[] { '_' })[0]).ToArray();
			var fps = (from personId in personIds
				select (from filename in Directory.GetFiles(templates, personId + "_*.xml").Take(5)
			                   select new FingerprintTemplate(XElement.Load(filename))).ToArray()).ToArray();
            var matching = new List<double>();
            var nonmatching = new List<double>();
            foreach (var row in fps)
                foreach (var probe in row)
                {
                    var matcher = new FingerprintMatcher(probe);
                    matching.AddRange(row.Where(c => c != probe).Select(matcher.Match));
                    nonmatching.AddRange(fps.Where(cr => cr != row).SelectMany(r => r).Select(matcher.Match));
                }
            matching.Sort();
            nonmatching.Sort();
			var both = matching.Concat(nonmatching).ToList();
			var thresholds = (from score in both.Concat(new[] { both.Max() + 1 }).Distinct().OrderBy(s => s)
			                  select new
			{
				Threshold = score,
				FMR = ApplyThreshold(nonmatching, score) / (double)nonmatching.Count,
				FNMR = (matching.Count - ApplyThreshold(matching, score)) / (double)matching.Count
			}).ToList();
			var eer = thresholds.First(t => t.FNMR >= t.FMR);
			Console.WriteLine("EER {0:%0.00} @ threshold {1:0.0}", (eer.FMR + eer.FNMR) / 2, eer.Threshold);
			var fmr1k = thresholds.First(t => t.FMR <= 0.001);
			Console.WriteLine("FMR1000 {0:%0.00} @ threshold {1:0.0}", fmr1k.FNMR, fmr1k.Threshold);
			var fmr10k = thresholds.First(t => t.FMR <= 0.0001);
			Console.WriteLine("FMR10000 {0:%0.00} @ threshold {1:0.0}", fmr10k.FNMR, fmr10k.Threshold);
		}

        static int ApplyThreshold(List<double> scores, double threshold)
        {
            var found = scores.BinarySearch(threshold);
            return scores.Count - (found >= 0 ? found : ~found);
        }
    }
}
