// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class DoublePointMatrixTest
    {
        DoublePointMatrix m;

        [SetUp]
        public void SetUp()
        {
            m = new DoublePointMatrix(4, 5);
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    m[x, y] = new DoublePoint(10 * x, 10 * y);
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(4, m.Width);
            Assert.AreEqual(5, m.Height);
        }
        [Test]
        public void ConstructorFromPoint()
        {
            var m = new DoublePointMatrix(new IntPoint(4, 5));
            Assert.AreEqual(4, m.Width);
            Assert.AreEqual(5, m.Height);
        }
        [Test]
        public void Get()
        {
            DoublePointTest.AssertPointEquals(new DoublePoint(20, 30), m[2, 3], 0.001);
            DoublePointTest.AssertPointEquals(new DoublePoint(30, 10), m[3, 1], 0.001);
        }
        [Test]
        public void GetAt()
        {
            DoublePointTest.AssertPointEquals(new DoublePoint(10, 20), m[new IntPoint(1, 2)], 0.001);
            DoublePointTest.AssertPointEquals(new DoublePoint(20, 40), m[new IntPoint(2, 4)], 0.001);
        }
        [Test]
        public void SetValues()
        {
            m.Set(2, 4, 101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), m[2, 4], 0.001);
        }
        [Test]
        public void Set()
        {
            m[1, 2] = new DoublePoint(101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), m[1, 2], 0.001);
        }
        [Test]
        public void SetAt()
        {
            m[new IntPoint(3, 2)] = new DoublePoint(101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), m[3, 2], 0.001);
        }
        [Test]
        public void AddValues()
        {
            m.Add(3, 1, 100, 200);
            DoublePointTest.AssertPointEquals(new DoublePoint(130, 210), m[3, 1], 0.001);
        }
        [Test]
        public void Add()
        {
            m.Add(2, 3, new DoublePoint(100, 200));
            DoublePointTest.AssertPointEquals(new DoublePoint(120, 230), m[2, 3], 0.001);
        }
        [Test]
        public void AddAt()
        {
            m.Add(new IntPoint(2, 4), new DoublePoint(100, 200));
            DoublePointTest.AssertPointEquals(new DoublePoint(120, 240), m[2, 4], 0.001);
        }
    }
}
