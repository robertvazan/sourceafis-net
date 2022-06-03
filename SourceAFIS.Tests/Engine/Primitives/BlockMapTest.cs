// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS.Engine.Primitives
{
    public class BlockMapTest
    {
        [Test]
        public void Constructor()
        {
            BlockMap m = new BlockMap(400, 600, 20);
            Assert.AreEqual(new IntPoint(400, 600), m.Pixels);
            Assert.AreEqual(new IntPoint(20, 30), m.Primary.Blocks);
            Assert.AreEqual(new IntPoint(21, 31), m.Primary.Corners);
            Assert.AreEqual(new IntPoint(21, 31), m.Secondary.Blocks);
            Assert.AreEqual(new IntPoint(22, 32), m.Secondary.Corners);
            Assert.AreEqual(new IntPoint(0, 0), m.Primary.Corner(0, 0));
            Assert.AreEqual(new IntPoint(400, 600), m.Primary.Corner(20, 30));
            Assert.AreEqual(new IntPoint(200, 300), m.Primary.Corner(10, 15));
            Assert.AreEqual(new IntRect(0, 0, 20, 20), m.Primary.Block(0, 0));
            Assert.AreEqual(new IntRect(380, 580, 20, 20), m.Primary.Block(19, 29));
            Assert.AreEqual(new IntRect(200, 300, 20, 20), m.Primary.Block(10, 15));
            Assert.AreEqual(new IntRect(0, 0, 10, 10), m.Secondary.Block(0, 0));
            Assert.AreEqual(new IntRect(390, 590, 10, 10), m.Secondary.Block(20, 30));
            Assert.AreEqual(new IntRect(190, 290, 20, 20), m.Secondary.Block(10, 15));
        }
    }
}
