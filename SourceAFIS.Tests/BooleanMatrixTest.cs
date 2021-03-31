// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class BooleanMatrixTest
    {
        BooleanMatrix M = new BooleanMatrix(4, 5);

        [SetUp]
        public void SetUp()
        {
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    M[x, y] = (x + y) % 2 > 0;
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
            var M = new BooleanMatrix(new IntPoint(4, 5));
            Assert.AreEqual(4, M.Width);
            Assert.AreEqual(5, M.Height);
        }
        [Test]
        public void ConstructorCloning()
        {
            var M = new BooleanMatrix(this.M);
            Assert.AreEqual(4, M.Width);
            Assert.AreEqual(5, M.Height);
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    Assert.AreEqual(this.M[x, y], M[x, y]);
        }
        [Test]
        public void Size()
        {
            Assert.AreEqual(4, M.Size.X);
            Assert.AreEqual(5, M.Size.Y);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(true, M[1, 4]);
            Assert.AreEqual(false, M[3, 1]);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(true, M[new IntPoint(3, 2)]);
            Assert.AreEqual(false, M[new IntPoint(2, 4)]);
        }
        [Test]
        public void GetFallback()
        {
            Assert.AreEqual(false, M.Get(0, 0, true));
            Assert.AreEqual(true, M.Get(3, 0, false));
            Assert.AreEqual(false, M.Get(0, 4, true));
            Assert.AreEqual(true, M.Get(3, 4, false));
            Assert.AreEqual(false, M.Get(-1, 4, false));
            Assert.AreEqual(true, M.Get(-1, 4, true));
            Assert.AreEqual(false, M.Get(2, -1, false));
            Assert.AreEqual(true, M.Get(4, 2, true));
            Assert.AreEqual(false, M.Get(2, 5, false));
        }
        [Test]
        public void GetAtFallback()
        {
            Assert.AreEqual(false, M.Get(new IntPoint(0, 0), true));
            Assert.AreEqual(true, M.Get(new IntPoint(3, 0), false));
            Assert.AreEqual(false, M.Get(new IntPoint(0, 4), true));
            Assert.AreEqual(true, M.Get(new IntPoint(3, 4), false));
            Assert.AreEqual(false, M.Get(new IntPoint(-1, 2), false));
            Assert.AreEqual(true, M.Get(new IntPoint(-1, 2), true));
            Assert.AreEqual(false, M.Get(new IntPoint(0, -1), false));
            Assert.AreEqual(true, M.Get(new IntPoint(4, 0), true));
            Assert.AreEqual(false, M.Get(new IntPoint(0, 5), false));
        }
        [Test]
        public void Set()
        {
            Assert.AreEqual(false, M[2, 4]);
            M[2, 4] = true;
            Assert.AreEqual(true, M[2, 4]);
        }
        [Test]
        public void SetAt()
        {
            Assert.AreEqual(true, M[1, 2]);
            M[new IntPoint(1, 2)] = false;
            Assert.AreEqual(false, M[1, 2]);
        }
        [Test]
        public void Invert()
        {
            M.Invert();
            Assert.AreEqual(true, M[0, 0]);
            Assert.AreEqual(false, M[3, 0]);
            Assert.AreEqual(true, M[0, 4]);
            Assert.AreEqual(false, M[3, 4]);
            Assert.AreEqual(true, M[1, 3]);
            Assert.AreEqual(false, M[2, 1]);
        }
        [Test]
        public void Merge()
        {
            Assert.AreEqual(true, M[3, 2]);
            var o = new BooleanMatrix(4, 5);
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    o[x, y] = x < 2 && y < 3;
            M.Merge(o);
            Assert.AreEqual(true, M[0, 0]);
            Assert.AreEqual(true, M[1, 2]);
            Assert.AreEqual(false, M[1, 3]);
            Assert.AreEqual(true, M[3, 2]);
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    Assert.AreEqual((x + y) % 2 > 0 || x < 2 && y < 3, M[x, y]);
        }
    }
}
