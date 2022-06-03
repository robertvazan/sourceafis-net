// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class BlockGridTest
    {
        BlockGrid g = new BlockGrid(3, 4);

        public BlockGridTest()
        {
            for (int i = 0; i < g.X.Length; ++i)
                g.X[i] = (i + 1) * 10;
            for (int i = 0; i < g.Y.Length; ++i)
                g.Y[i] = (i + 1) * 100;
        }

        [Test]
        public void Constructor()
        {
            Assert.AreEqual(4, g.X.Length);
            Assert.AreEqual(5, g.Y.Length);
        }
        [Test]
        public void ConstructorFromPoint()
        {
            var g = new BlockGrid(new IntPoint(2, 3));
            Assert.AreEqual(3, g.X.Length);
            Assert.AreEqual(4, g.Y.Length);
        }
        [Test]
        public void CornerXY() => Assert.AreEqual(new IntPoint(20, 300), g.Corner(1, 2));
        [Test]
        public void CornerAt() => Assert.AreEqual(new IntPoint(10, 200), g.Corner(new IntPoint(0, 1)));
        [Test]
        public void BlockXY() => Assert.AreEqual(new IntRect(20, 300, 10, 100), g.Block(1, 2));
        [Test]
        public void BlockAt() => Assert.AreEqual(new IntRect(10, 200, 10, 100), g.Block(new IntPoint(0, 1)));
    }
}
