// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class DoubleAngleTest
    {
        [Test]
        public void ToVector()
        {
            DoublePointTest.AssertPointEquals(new DoublePoint(1, 0), DoubleAngle.ToVector(-DoubleAngle.Pi2), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, 1), DoubleAngle.ToVector(-1.5 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(-1, 0), DoubleAngle.ToVector(-Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, -1), DoubleAngle.ToVector(-0.5 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(1, 0), DoubleAngle.ToVector(0), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(Math.Sqrt(2) / 2, Math.Sqrt(2) / 2), DoubleAngle.ToVector(Math.PI / 4), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, 1), DoubleAngle.ToVector(Math.PI / 2), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(-1, 0), DoubleAngle.ToVector(Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, -1), DoubleAngle.ToVector(1.5 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(1, 0), DoubleAngle.ToVector(DoubleAngle.Pi2), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, 1), DoubleAngle.ToVector(2.5 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(-1, 0), DoubleAngle.ToVector(3 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(0, -1), DoubleAngle.ToVector(3.5 * Math.PI), 0.01);
            DoublePointTest.AssertPointEquals(new DoublePoint(1, 0), DoubleAngle.ToVector(2 * DoubleAngle.Pi2), 0.01);
        }
        [Test]
        public void Atan()
        {
            Assert.AreEqual(0, DoubleAngle.Atan(new DoublePoint(5, 0)), 0.001);
            Assert.AreEqual(0.25 * Math.PI, DoubleAngle.Atan(new DoublePoint(1, 1)), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Atan(new DoublePoint(0, 3)), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Atan(new DoublePoint(-0.3, 0)), 0.001);
            Assert.AreEqual(1.5 * Math.PI, DoubleAngle.Atan(new DoublePoint(0, -1)), 0.001);
            Assert.AreEqual(1.75 * Math.PI, DoubleAngle.Atan(new DoublePoint(1, -1)), 0.001);
        }
        [Test]
        public void AtanPoint() => Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Atan(new IntPoint(0, 2)), 0.001);
        [Test]
        public void AtanCenter() => Assert.AreEqual(0.25 * Math.PI, DoubleAngle.Atan(new IntPoint(2, 3), new IntPoint(4, 5)), 0.001);
        [Test]
        public void ToOrientation()
        {
            Assert.AreEqual(0, DoubleAngle.ToOrientation(0), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.ToOrientation(0.25 * Math.PI), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.ToOrientation(0.5 * Math.PI), 0.001);
            Assert.AreEqual(2 * Math.PI, DoubleAngle.ToOrientation(Math.PI - 0.000001), 0.001);
            Assert.AreEqual(0, DoubleAngle.ToOrientation(Math.PI + 0.000001), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.ToOrientation(1.5 * Math.PI), 0.001);
            Assert.AreEqual(1.5 * Math.PI, DoubleAngle.ToOrientation(1.75 * Math.PI), 0.001);
            Assert.AreEqual(2 * Math.PI, DoubleAngle.ToOrientation(2 * Math.PI - 0.000001), 0.001);
        }
        [Test]
        public void Add()
        {
            Assert.AreEqual(0, DoubleAngle.Add(0, 0), 0.001);
            Assert.AreEqual(0.75 * Math.PI, DoubleAngle.Add(0.25 * Math.PI, 0.5 * Math.PI), 0.001);
            Assert.AreEqual(1.75 * Math.PI, DoubleAngle.Add(Math.PI, 0.75 * Math.PI), 0.001);
            Assert.AreEqual(0.25 * Math.PI, DoubleAngle.Add(Math.PI, 1.25 * Math.PI), 0.001);
            Assert.AreEqual(1.5 * Math.PI, DoubleAngle.Add(1.75 * Math.PI, 1.75 * Math.PI), 0.001);
        }
        [Test]
        public void Opposite()
        {
            Assert.AreEqual(Math.PI, DoubleAngle.Opposite(0), 0.001);
            Assert.AreEqual(1.25 * Math.PI, DoubleAngle.Opposite(0.25 * Math.PI), 0.001);
            Assert.AreEqual(1.5 * Math.PI, DoubleAngle.Opposite(0.5 * Math.PI), 0.001);
            Assert.AreEqual(2 * Math.PI, DoubleAngle.Opposite(Math.PI - 0.000001), 0.001);
            Assert.AreEqual(0, DoubleAngle.Opposite(Math.PI + 0.000001), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Opposite(1.5 * Math.PI), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Opposite(2 * Math.PI - 0.000001), 0.001);
        }
        [Test]
        public void Distance()
        {
            Assert.AreEqual(Math.PI, DoubleAngle.Distance(0, Math.PI), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Distance(1.5 * Math.PI, 0.5 * Math.PI), 0.001);
            Assert.AreEqual(0.75 * Math.PI, DoubleAngle.Distance(0.75 * Math.PI, 1.5 * Math.PI), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Distance(0.25 * Math.PI, 1.75 * Math.PI), 0.001);
        }
        [Test]
        public void Difference()
        {
            Assert.AreEqual(Math.PI, DoubleAngle.Difference(0, Math.PI), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Difference(1.5 * Math.PI, 0.5 * Math.PI), 0.001);
            Assert.AreEqual(1.25 * Math.PI, DoubleAngle.Difference(0.75 * Math.PI, 1.5 * Math.PI), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Difference(0.25 * Math.PI, 1.75 * Math.PI), 0.001);
        }
        [Test]
        public void Complementary()
        {
            Assert.AreEqual(0, DoubleAngle.Complementary(0), 0.001);
            Assert.AreEqual(1.5 * Math.PI, DoubleAngle.Complementary(0.5 * Math.PI), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Complementary(Math.PI - 0.0000001), 0.001);
            Assert.AreEqual(Math.PI, DoubleAngle.Complementary(Math.PI + 0.0000001), 0.001);
            Assert.AreEqual(0.5 * Math.PI, DoubleAngle.Complementary(1.5 * Math.PI), 0.001);
            Assert.AreEqual(0, DoubleAngle.Complementary(2 * Math.PI - 0.0000001), 0.001);
        }
        [Test]
        public void BucketCenter()
        {
            Assert.AreEqual(0.25 * Math.PI, DoubleAngle.BucketCenter(0, 4), 0.001);
            Assert.AreEqual(0.75 * Math.PI, DoubleAngle.BucketCenter(1, 4), 0.001);
            Assert.AreEqual(1.25 * Math.PI, DoubleAngle.BucketCenter(2, 4), 0.001);
            Assert.AreEqual(1.75 * Math.PI, DoubleAngle.BucketCenter(3, 4), 0.001);
        }
        [Test]
        public void Quantize()
        {
            Assert.AreEqual(0, DoubleAngle.Quantize(-0.0001, 4));
            Assert.AreEqual(0, DoubleAngle.Quantize(0, 4));
            Assert.AreEqual(0, DoubleAngle.Quantize(0.25 * Math.PI, 4));
            Assert.AreEqual(2, DoubleAngle.Quantize(Math.PI, 5));
            Assert.AreEqual(6, DoubleAngle.Quantize(1.75 * Math.PI, 7));
            Assert.AreEqual(9, DoubleAngle.Quantize(DoubleAngle.Pi2 - 0.001, 10));
            Assert.AreEqual(9, DoubleAngle.Quantize(DoubleAngle.Pi2 + 0.001, 10));
        }
    }
}
