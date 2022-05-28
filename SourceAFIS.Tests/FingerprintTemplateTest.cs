// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using NUnit.Framework;
using SourceAFIS.Features;
using SourceAFIS.Primitives;
using SourceAFIS.Templates;

namespace SourceAFIS
{
    public class FingerprintTemplateTest
    {
        public static FingerprintTemplate Probe() => new FingerprintTemplate(FingerprintImageTest.Probe());
        public static FingerprintTemplate Matching() => new FingerprintTemplate(FingerprintImageTest.Matching());
        public static FingerprintTemplate Nonmatching() => new FingerprintTemplate(FingerprintImageTest.Nonmatching());
        public static FingerprintTemplate ProbeGray() => new FingerprintTemplate(FingerprintImageTest.ProbeGray());
        public static FingerprintTemplate MatchingGray() => new FingerprintTemplate(FingerprintImageTest.MatchingGray());
        public static FingerprintTemplate NonmatchingGray() => new FingerprintTemplate(FingerprintImageTest.NonmatchingGray());

        [Test]
        public void Constructor() => Probe();
        [Test]
        public void RoundTripSerialization()
        {
            var mt = new MutableTemplate();
            mt.Size = new IntPoint(800, 600);
            mt.Minutiae = new List<MutableMinutia>();
            mt.Minutiae.Add(new MutableMinutia(new IntPoint(100, 200), Math.PI, MinutiaType.Bifurcation));
            mt.Minutiae.Add(new MutableMinutia(new IntPoint(300, 400), 0.5 * Math.PI, MinutiaType.Ending));
            var pt = new PersistentTemplate(mt);
            var t = new FingerprintTemplate(SerializationUtils.Serialize(pt));
            t = new FingerprintTemplate(t.ToByteArray());
            Assert.AreEqual(2, t.Minutiae.Length);
            var a = t.Minutiae[0];
            var b = t.Minutiae[1];
            Assert.AreEqual(new IntPoint(100, 200), a.Position);
            Assert.AreEqual(Math.PI, a.Direction, 0.0000001);
            Assert.AreEqual(MinutiaType.Bifurcation, a.Type);
            Assert.AreEqual(new IntPoint(300, 400), b.Position);
            Assert.AreEqual(0.5 * Math.PI, b.Direction, 0.0000001);
            Assert.AreEqual(MinutiaType.Ending, b.Type);
        }
    }
}
