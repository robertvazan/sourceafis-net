// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class DoubleMatrixTest
    {
        DoubleMatrix m;

        [SetUp]
        public void SetUp()
        {
            m = new DoubleMatrix(3, 4);
            for (int x = 0; x < m.Width; ++x)
                for (int y = 0; y < m.Height; ++y)
                    m[x, y] = 10 * x + y;
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(3, m.Width);
            Assert.AreEqual(4, m.Height);
        }
        [Test]
        public void ConstructorFromPoint()
        {
            var m = new DoubleMatrix(new IntPoint(3, 4));
            Assert.AreEqual(3, m.Width);
            Assert.AreEqual(4, m.Height);
        }
        [Test]
        public void Size()
        {
            Assert.AreEqual(3, m.Size.X);
            Assert.AreEqual(4, m.Size.Y);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(12, m[1, 2], 0.001);
            Assert.AreEqual(21, m[2, 1], 0.001);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(3, m[new IntPoint(0, 3)], 0.001);
            Assert.AreEqual(22, m[new IntPoint(2, 2)], 0.001);
        }
        [Test]
        public void Set()
        {
            m[1, 2] = 101;
            Assert.AreEqual(101, m[1, 2], 0.001);
        }
        [Test]
        public void SetAt()
        {
            m[new IntPoint(2, 3)] = 101;
            Assert.AreEqual(101, m[2, 3], 0.001);
        }
        [Test]
        public void Add()
        {
            m.Add(2, 1, 100);
            Assert.AreEqual(121, m[2, 1], 0.001);
        }
        [Test]
        public void AddAt()
        {
            m.Add(new IntPoint(2, 3), 100);
            Assert.AreEqual(123, m[2, 3], 0.001);
        }
        [Test]
        public void Multiply()
        {
            m.Multiply(1, 3, 10);
            Assert.AreEqual(130, m[1, 3], 0.001);
        }
        [Test]
        public void MultiplyAt()
        {
            m.Multiply(new IntPoint(1, 2), 10);
            Assert.AreEqual(120, m[1, 2], 0.001);
        }
    }
}
