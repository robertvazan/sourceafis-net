// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;
using SourceAFIS.Engine.Templates;

// TODO: Port randomScaleMatch() from Java.
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
            var mt = new FeatureTemplate(new(800, 600), new());
            mt.Minutiae.Add(new(new(100, 200), FloatAngle.Pi, MinutiaType.Bifurcation));
            mt.Minutiae.Add(new(new(300, 400), FloatAngle.HalfPi, MinutiaType.Ending));
            var pt = new PersistentTemplate(mt);
            var t = new FingerprintTemplate(SerializationUtils.Serialize(pt));
            t = new FingerprintTemplate(t.ToByteArray());
            Assert.AreEqual(2, t.Minutiae.Length);
            var a = t.Minutiae[0];
            var b = t.Minutiae[1];
            Assert.AreEqual(new ShortPoint(100, 200), a.Position);
            Assert.AreEqual(Math.PI, a.Direction, 0.0000001);
            Assert.AreEqual(MinutiaType.Bifurcation, a.Type);
            Assert.AreEqual(new ShortPoint(300, 400), b.Position);
            Assert.AreEqual(0.5 * Math.PI, b.Direction, 0.0000001);
            Assert.AreEqual(MinutiaType.Ending, b.Type);
        }
    }
}
