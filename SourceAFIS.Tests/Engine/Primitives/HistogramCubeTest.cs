// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class HistogramCubeTest
    {
        HistogramCube h;

        [SetUp]
        public void SetUp()
        {
            h = new HistogramCube(4, 5, 6);
            for (int x = 0; x < h.Width; ++x)
                for (int y = 0; y < h.Height; ++y)
                    for (int z = 0; z < h.Bins; ++z)
                        h[x, y, z] = 100 * x + 10 * y + z;
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(4, h.Width);
            Assert.AreEqual(5, h.Height);
            Assert.AreEqual(6, h.Bins);
        }
        [Test]
        public void Constrain()
        {
            Assert.AreEqual(3, h.Constrain(3));
            Assert.AreEqual(0, h.Constrain(0));
            Assert.AreEqual(5, h.Constrain(5));
            Assert.AreEqual(0, h.Constrain(-1));
            Assert.AreEqual(5, h.Constrain(6));
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(234, h[2, 3, 4]);
            Assert.AreEqual(312, h[3, 1, 2]);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(125, h[new IntPoint(1, 2), 5]);
            Assert.AreEqual(243, h[new IntPoint(2, 4), 3]);
        }
        [Test]
        public void Sum()
        {
            Assert.AreEqual(6 * 120 + 1 + 2 + 3 + 4 + 5, h.Sum(1, 2));
        }
        [Test]
        public void SumAt()
        {
            Assert.AreEqual(6 * 340 + 1 + 2 + 3 + 4 + 5, h.Sum(new IntPoint(3, 4)));
        }
        [Test]
        public void Set()
        {
            h[2, 4, 3] = 1000;
            Assert.AreEqual(1000, h[2, 4, 3]);
        }
        [Test]
        public void SetAt()
        {
            h[new IntPoint(3, 1), 5] = 1000;
            Assert.AreEqual(1000, h[3, 1, 5]);
        }
        [Test]
        public void Add()
        {
            h.Add(1, 2, 4, 1000);
            Assert.AreEqual(1124, h[1, 2, 4]);
        }
        [Test]
        public void AddAt()
        {
            h.Add(new IntPoint(2, 4), 1, 1000);
            Assert.AreEqual(1241, h[2, 4, 1]);
        }
        [Test]
        public void Increment()
        {
            h.Increment(3, 4, 1);
            Assert.AreEqual(342, h[3, 4, 1]);
        }
        [Test]
        public void IncrementAt()
        {
            h.Increment(new IntPoint(2, 3), 5);
            Assert.AreEqual(236, h[2, 3, 5]);
        }
    }
}
