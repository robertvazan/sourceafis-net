// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class DoublePointTest
    {
        [Test]
        public void Constructor()
        {
            var p = new DoublePoint(2.5, 3.5);
            Assert.AreEqual(2.5, p.X, 0.001);
            Assert.AreEqual(3.5, p.Y, 0.001);
        }
        [Test]
        public void Plus() => AssertPointEquals(new DoublePoint(6, 8), new DoublePoint(2, 3) + new DoublePoint(4, 5), 0.001);
        [Test]
        public void Multiply() => AssertPointEquals(new DoublePoint(1, 1.5), 0.5 * new DoublePoint(2, 3), 0.001);
        [Test]
        public void Round()
        {
            Assert.AreEqual(new IntPoint(2, 3), new DoublePoint(2.4, 2.6).Round());
            Assert.AreEqual(new IntPoint(-2, -3), new DoublePoint(-2.4, -2.6).Round());
        }
        internal static void AssertPointEquals(DoublePoint expected, DoublePoint actual, double tolerance)
        {
            Assert.AreEqual(expected.X, actual.X, tolerance);
            Assert.AreEqual(expected.Y, actual.Y, tolerance);
        }
    }
}
