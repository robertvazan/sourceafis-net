// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class DoubleMatrixTest
    {
        DoubleMatrix M;

        [SetUp]
        public void SetUp()
        {
            M = new DoubleMatrix(3, 4);
            for (int x = 0; x < M.Width; ++x)
                for (int y = 0; y < M.Height; ++y)
                    M[x, y] = 10 * x + y;
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(3, M.Width);
            Assert.AreEqual(4, M.Height);
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
            Assert.AreEqual(3, M.Size.X);
            Assert.AreEqual(4, M.Size.Y);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(12, M[1, 2], 0.001);
            Assert.AreEqual(21, M[2, 1], 0.001);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(3, M[new IntPoint(0, 3)], 0.001);
            Assert.AreEqual(22, M[new IntPoint(2, 2)], 0.001);
        }
        [Test]
        public void Set()
        {
            M[1, 2] = 101;
            Assert.AreEqual(101, M[1, 2], 0.001);
        }
        [Test]
        public void SetAt()
        {
            M[new IntPoint(2, 3)] = 101;
            Assert.AreEqual(101, M[2, 3], 0.001);
        }
        [Test]
        public void Add()
        {
            M.Add(2, 1, 100);
            Assert.AreEqual(121, M[2, 1], 0.001);
        }
        [Test]
        public void AddAt()
        {
            M.Add(new IntPoint(2, 3), 100);
            Assert.AreEqual(123, M[2, 3], 0.001);
        }
        [Test]
        public void Multiply()
        {
            M.Multiply(1, 3, 10);
            Assert.AreEqual(130, M[1, 3], 0.001);
        }
        [Test]
        public void MultiplyAt()
        {
            M.Multiply(new IntPoint(1, 2), 10);
            Assert.AreEqual(120, M[1, 2], 0.001);
        }
    }
}
