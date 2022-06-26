// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS
{
    public class FingerprintImageTest
    {
        [Test]
        public void DecodePng() => new FingerprintImage(TestResources.Png());

        void AssertSimilar(DoubleMatrix matrix, DoubleMatrix reference)
        {
            Assert.AreEqual(reference.Width, matrix.Width);
            Assert.AreEqual(reference.Height, matrix.Height);
            double delta = 0, max = -1, min = 1;
            for (int x = 0; x < matrix.Width; ++x)
            {
                for (int y = 0; y < matrix.Height; ++y)
                {
                    delta += Math.Abs(matrix[x, y] - reference[x, y]);
                    max = Math.Max(max, matrix[x, y]);
                    min = Math.Min(min, matrix[x, y]);
                }
            }
            Assert.IsTrue(max > 0.75);
            Assert.IsTrue(min < 0.1);
            Assert.IsTrue(delta / (matrix.Width * matrix.Height) < 0.01);
        }
        void AssertSimilar(byte[] image, byte[] reference) => AssertSimilar(new FingerprintImage(image).Matrix, new FingerprintImage(reference).Matrix);

        [Test]
        public void DecodeJpeg() => AssertSimilar(TestResources.Jpeg(), TestResources.Png());
        [Test]
        public void DecodeBmp() => AssertSimilar(TestResources.Bmp(), TestResources.Png());

        public static FingerprintImage Probe() => new FingerprintImage(TestResources.Probe());
        public static FingerprintImage Matching() => new FingerprintImage(TestResources.Matching());
        public static FingerprintImage Nonmatching() => new FingerprintImage(TestResources.Nonmatching());
        public static FingerprintImage ProbeGray() => new FingerprintImage(332, 533, TestResources.ProbeGray());
        public static FingerprintImage MatchingGray() => new FingerprintImage(320, 407, TestResources.MatchingGray());
        public static FingerprintImage NonmatchingGray() => new FingerprintImage(333, 435, TestResources.NonmatchingGray());

        [Test]
        public void DecodeGray()
        {
            double score = new FingerprintMatcher(new FingerprintTemplate(ProbeGray()))
                .Match(new FingerprintTemplate(MatchingGray()));
            Assert.That(score, Is.GreaterThan(40));
        }
    }
}
