// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class DoublePointMatrixTest
    {
        DoublePointMatrix M;

        [SetUp]
        public void SetUp()
        {
            M = new DoublePointMatrix(4, 5);
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    M[x, y] = new DoublePoint(10 * x, 10 * y);
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(4, M.Width);
            Assert.AreEqual(5, M.Height);
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
            DoublePointTest.AssertPointEquals(new DoublePoint(20, 30), M[2, 3], 0.001);
            DoublePointTest.AssertPointEquals(new DoublePoint(30, 10), M[3, 1], 0.001);
        }
        [Test]
        public void GetAt()
        {
            DoublePointTest.AssertPointEquals(new DoublePoint(10, 20), M[new IntPoint(1, 2)], 0.001);
            DoublePointTest.AssertPointEquals(new DoublePoint(20, 40), M[new IntPoint(2, 4)], 0.001);
        }
        [Test]
        public void SetValues()
        {
            M.Set(2, 4, 101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), M[2, 4], 0.001);
        }
        [Test]
        public void Set()
        {
            M[1, 2] = new DoublePoint(101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), M[1, 2], 0.001);
        }
        [Test]
        public void SetAt()
        {
            M[new IntPoint(3, 2)] = new DoublePoint(101, 102);
            DoublePointTest.AssertPointEquals(new DoublePoint(101, 102), M[3, 2], 0.001);
        }
        [Test]
        public void AddValues()
        {
            M.Add(3, 1, 100, 200);
            DoublePointTest.AssertPointEquals(new DoublePoint(130, 210), M[3, 1], 0.001);
        }
        [Test]
        public void Add()
        {
            M.Add(2, 3, new DoublePoint(100, 200));
            DoublePointTest.AssertPointEquals(new DoublePoint(120, 230), M[2, 3], 0.001);
        }
        [Test]
        public void AddAt()
        {
            M.Add(new IntPoint(2, 4), new DoublePoint(100, 200));
            DoublePointTest.AssertPointEquals(new DoublePoint(120, 240), M[2, 4], 0.001);
        }
    }
}
