// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class BooleanMatrixTest
    {
        BooleanMatrix m = new BooleanMatrix(4, 5);

        [SetUp]
        public void SetUp()
        {
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    m[x, y] = (x + y) % 2 > 0;
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
            var m = new BooleanMatrix(new IntPoint(4, 5));
            Assert.AreEqual(4, m.Width);
            Assert.AreEqual(5, m.Height);
        }
        [Test]
        public void ConstructorCloning()
        {
            var m = new BooleanMatrix(this.m);
            Assert.AreEqual(4, m.Width);
            Assert.AreEqual(5, m.Height);
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    Assert.AreEqual(this.m[x, y], m[x, y]);
        }
        [Test]
        public void Size()
        {
            Assert.AreEqual(4, m.Size.X);
            Assert.AreEqual(5, m.Size.Y);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(true, m[1, 4]);
            Assert.AreEqual(false, m[3, 1]);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(true, m[new IntPoint(3, 2)]);
            Assert.AreEqual(false, m[new IntPoint(2, 4)]);
        }
        [Test]
        public void GetFallback()
        {
            Assert.AreEqual(false, m.Get(0, 0, true));
            Assert.AreEqual(true, m.Get(3, 0, false));
            Assert.AreEqual(false, m.Get(0, 4, true));
            Assert.AreEqual(true, m.Get(3, 4, false));
            Assert.AreEqual(false, m.Get(-1, 4, false));
            Assert.AreEqual(true, m.Get(-1, 4, true));
            Assert.AreEqual(false, m.Get(2, -1, false));
            Assert.AreEqual(true, m.Get(4, 2, true));
            Assert.AreEqual(false, m.Get(2, 5, false));
        }
        [Test]
        public void GetAtFallback()
        {
            Assert.AreEqual(false, m.Get(new IntPoint(0, 0), true));
            Assert.AreEqual(true, m.Get(new IntPoint(3, 0), false));
            Assert.AreEqual(false, m.Get(new IntPoint(0, 4), true));
            Assert.AreEqual(true, m.Get(new IntPoint(3, 4), false));
            Assert.AreEqual(false, m.Get(new IntPoint(-1, 2), false));
            Assert.AreEqual(true, m.Get(new IntPoint(-1, 2), true));
            Assert.AreEqual(false, m.Get(new IntPoint(0, -1), false));
            Assert.AreEqual(true, m.Get(new IntPoint(4, 0), true));
            Assert.AreEqual(false, m.Get(new IntPoint(0, 5), false));
        }
        [Test]
        public void Set()
        {
            Assert.AreEqual(false, m[2, 4]);
            m[2, 4] = true;
            Assert.AreEqual(true, m[2, 4]);
        }
        [Test]
        public void SetAt()
        {
            Assert.AreEqual(true, m[1, 2]);
            m[new IntPoint(1, 2)] = false;
            Assert.AreEqual(false, m[1, 2]);
        }
        [Test]
        public void Invert()
        {
            m.Invert();
            Assert.AreEqual(true, m[0, 0]);
            Assert.AreEqual(false, m[3, 0]);
            Assert.AreEqual(true, m[0, 4]);
            Assert.AreEqual(false, m[3, 4]);
            Assert.AreEqual(true, m[1, 3]);
            Assert.AreEqual(false, m[2, 1]);
        }
        [Test]
        public void Merge()
        {
            Assert.AreEqual(true, m[3, 2]);
            var o = new BooleanMatrix(4, 5);
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    o[x, y] = x < 2 && y < 3;
            m.Merge(o);
            Assert.AreEqual(true, m[0, 0]);
            Assert.AreEqual(true, m[1, 2]);
            Assert.AreEqual(false, m[1, 3]);
            Assert.AreEqual(true, m[3, 2]);
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    Assert.AreEqual((x + y) % 2 > 0 || x < 2 && y < 3, m[x, y]);
        }
    }
}
