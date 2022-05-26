// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class HistogramCubeTest
    {
        HistogramCube H;

        [SetUp]
        public void SetUp()
        {
            H = new HistogramCube(4, 5, 6);
            for (int x = 0; x < H.Width; ++x)
                for (int y = 0; y < H.Height; ++y)
                    for (int z = 0; z < H.Bins; ++z)
                        H[x, y, z] = 100 * x + 10 * y + z;
        }
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(4, H.Width);
            Assert.AreEqual(5, H.Height);
            Assert.AreEqual(6, H.Bins);
        }
        [Test]
        public void Constrain()
        {
            Assert.AreEqual(3, H.Constrain(3));
            Assert.AreEqual(0, H.Constrain(0));
            Assert.AreEqual(5, H.Constrain(5));
            Assert.AreEqual(0, H.Constrain(-1));
            Assert.AreEqual(5, H.Constrain(6));
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(234, H[2, 3, 4]);
            Assert.AreEqual(312, H[3, 1, 2]);
        }
        [Test]
        public void GetAt()
        {
            Assert.AreEqual(125, H[new IntPoint(1, 2), 5]);
            Assert.AreEqual(243, H[new IntPoint(2, 4), 3]);
        }
        [Test]
        public void Sum()
        {
            Assert.AreEqual(6 * 120 + 1 + 2 + 3 + 4 + 5, H.Sum(1, 2));
        }
        [Test]
        public void SumAt()
        {
            Assert.AreEqual(6 * 340 + 1 + 2 + 3 + 4 + 5, H.Sum(new IntPoint(3, 4)));
        }
        [Test]
        public void Set()
        {
            H[2, 4, 3] = 1000;
            Assert.AreEqual(1000, H[2, 4, 3]);
        }
        [Test]
        public void SetAt()
        {
            H[new IntPoint(3, 1), 5] = 1000;
            Assert.AreEqual(1000, H[3, 1, 5]);
        }
        [Test]
        public void Add()
        {
            H.Add(1, 2, 4, 1000);
            Assert.AreEqual(1124, H[1, 2, 4]);
        }
        [Test]
        public void AddAt()
        {
            H.Add(new IntPoint(2, 4), 1, 1000);
            Assert.AreEqual(1241, H[2, 4, 1]);
        }
        [Test]
        public void Increment()
        {
            H.Increment(3, 4, 1);
            Assert.AreEqual(342, H[3, 4, 1]);
        }
        [Test]
        public void IncrementAt()
        {
            H.Increment(new IntPoint(2, 3), 5);
            Assert.AreEqual(236, H[2, 3, 5]);
        }
    }
}
