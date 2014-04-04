using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

namespace SourceAFIS.Tests
{
    [TestFixture]
    public class PerformanceTest
    {
        [Test]
        public void Extract()
        {
            var db = Path.Combine("FVC2000", "DB1_B");
			Console.WriteLine("Extracting database {0}", db);
            var images = Path.Combine(TestUtils.ImagePath, db);
            var templates = Path.Combine(TestUtils.TemplatePath, db);
            Directory.CreateDirectory(templates);
            var stopwatch = new Stopwatch();
            int count = 0;
            long size = 0;
            foreach (var file in Directory.GetFiles(images))
                if (TestUtils.ImageExtensions.Contains(Path.GetExtension(file)))
                {
                    var image = TestUtils.LoadImage(file);
                    stopwatch.Start();
                    var fp = new FingerprintTemplate(image);
                    stopwatch.Stop();
                    var template = Path.Combine(templates, Path.GetFileNameWithoutExtension(file) + ".xml");
                    fp.ToXml().Save(template);
                    size += new FileInfo(template).Length;
                    ++count;
                }
            Console.WriteLine("{0:0}ms extraction time, {1:0.0}KB templates", stopwatch.Elapsed.TotalMilliseconds / count, size / (double)count / 1024);
        }

		[Test]
		public void Match()
		{
			var db = Path.Combine("FVC2000", "DB1_B");
			Console.WriteLine("Matching database {0}", db);
			var templates = Path.Combine(TestUtils.TemplatePath, db);
			var personIds = (from filename in Directory.GetFiles(templates)
				select Path.GetFileNameWithoutExtension(filename).Split(new[] { '_' })[0]).Distinct().ToArray();
			var fps = (from personId in personIds
				select (from filename in Directory.GetFiles(templates, personId + "_*.xml")
			         select new FingerprintTemplate(XElement.Load(filename))).ToArray()).ToArray();
            var setupTime = new TimeBenchmark();
            var matchTime = new TimeBenchmark();
            var nonmatchTime = new TimeBenchmark();
            var matching = new List<double>();
            var nonmatching = new List<double>();
            foreach (var row in fps)
                foreach (var probe in row)
                {
                    var matcher = setupTime.Measure(() => new FingerprintMatcher(probe));
                    matching.AddRange(matchTime.Measure(row.Where(c => c != probe), matcher.Match));
                    nonmatching.AddRange(nonmatchTime.Measure(fps.Where(cr => cr != row).SelectMany(r => r), matcher.Match));
                }
            Console.WriteLine("{0:0} fp/s, {1:0.00}ms setup time, {2:0.00}ms final match time", nonmatchTime.Throughput, setupTime.UnitTime.TotalMilliseconds, matchTime.UnitTime.TotalMilliseconds);
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
			Console.WriteLine("EER {0:0.00%} @ threshold {1:0.0}", (eer.FMR + eer.FNMR) / 2, eer.Threshold);
			var fmr1k = thresholds.First(t => t.FMR <= 0.001);
			Console.WriteLine("FMR1000 {0:0.00%} @ threshold {1:0.0}", fmr1k.FNMR, fmr1k.Threshold);
			var fmr10k = thresholds.First(t => t.FMR <= 0.0001);
			Console.WriteLine("FMR10000 {0:0.00%} @ threshold {1:0.0}", fmr10k.FNMR, fmr10k.Threshold);
		}

        static int ApplyThreshold(List<double> scores, double threshold)
        {
            var found = scores.BinarySearch(threshold);
            return scores.Count - (found >= 0 ? found : ~found);
        }

        class TimeBenchmark
        {
            int Iterations;
            Stopwatch Stopwatch = new Stopwatch();

            public TimeSpan UnitTime { get { return new TimeSpan(Stopwatch.Elapsed.Ticks / Iterations); } }
            public double Throughput { get { return Iterations / Stopwatch.Elapsed.TotalSeconds; } }

            public T Measure<T>(Func<T> func)
            {
                Stopwatch.Start();
                var result = func();
                Stopwatch.Stop();
                ++Iterations;
                return result;
            }

            public IEnumerable<U> Measure<T, U>(IEnumerable<T> items, Func<T, U> func)
            {
                var list = items.ToList();
                foreach (var item in list)
                {
                    Stopwatch.Start();
                    var result = func(item);
                    Stopwatch.Stop();
                    ++Iterations;
                    yield return result;
                }
            }
        }
    }
}
