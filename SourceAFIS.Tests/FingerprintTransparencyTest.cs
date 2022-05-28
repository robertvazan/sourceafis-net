// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS
{
    public class FingerprintTransparencyTest
    {
        class TransparencyChecker : FingerprintTransparency
        {
            public readonly List<string> Keys = new List<string>();

            public override void Take(string key, string mime, byte[] data)
            {
                Keys.Add(key);
                Assert.Contains(mime, new[] { "application/cbor", "text/plain" });
                Assert.Greater(data.Length, 0);
            }
        }

        class TransparencyFilter : TransparencyChecker
        {
            public override bool Accepts(string key) => false;
        }

        [Test]
        public void Versioned()
        {
            using (var transparency = new TransparencyChecker())
            {
                FingerprintTemplateTest.ProbeGray();
                Assert.Contains("version", transparency.Keys);
            }
        }
        [Test]
        public void Extractor()
        {
            using (var transparency = new TransparencyChecker())
            {
                FingerprintTemplateTest.ProbeGray();
                Assert.IsNotEmpty(transparency.Keys);
            }
        }
        [Test]
        public void Matcher()
        {
            var probe = FingerprintTemplateTest.ProbeGray();
            var matching = FingerprintTemplateTest.MatchingGray();
            using (var transparency = new TransparencyChecker())
            {
                new FingerprintMatcher(probe).Match(matching);
                Assert.IsNotEmpty(transparency.Keys);
            }
        }
        [Test]
        public void Deserialization()
        {
            var serialized = FingerprintTemplateTest.ProbeGray().ToByteArray();
            using (var transparency = new TransparencyChecker())
            {
                new FingerprintTemplate(serialized);
                Assert.IsNotEmpty(transparency.Keys);
            }
        }
        [Test]
        public void Filtered()
        {
            using (var transparency = new TransparencyFilter())
            {
                new FingerprintMatcher(FingerprintTemplateTest.ProbeGray())
                    .Match(FingerprintTemplateTest.MatchingGray());
                Assert.IsEmpty(transparency.Keys);
            }
        }
    }
}
