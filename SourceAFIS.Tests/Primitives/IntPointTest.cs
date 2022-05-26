// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class IntPointTest
    {
        [Test]
        public void Constructor()
        {
            IntPoint p = new IntPoint(2, 3);
            Assert.AreEqual(2, p.X);
            Assert.AreEqual(3, p.Y);
        }
        [Test]
        public void Area()
        {
            Assert.AreEqual(6, new IntPoint(2, 3).Area);
        }
        [Test]
        public void LengthSq()
        {
            Assert.AreEqual(5 * 5, new IntPoint(3, 4).LengthSq);
            Assert.AreEqual(5 * 5, new IntPoint(-3, -4).LengthSq);
        }
        [Test]
        public void Contains()
        {
            IntPoint p = new IntPoint(3, 4);
            Assert.IsTrue(p.Contains(new IntPoint(1, 1)));
            Assert.IsTrue(p.Contains(new IntPoint(0, 0)));
            Assert.IsTrue(p.Contains(new IntPoint(2, 3)));
            Assert.IsTrue(p.Contains(new IntPoint(0, 3)));
            Assert.IsTrue(p.Contains(new IntPoint(2, 0)));
            Assert.IsFalse(p.Contains(new IntPoint(-1, 1)));
            Assert.IsFalse(p.Contains(new IntPoint(1, -1)));
            Assert.IsFalse(p.Contains(new IntPoint(-2, -3)));
            Assert.IsFalse(p.Contains(new IntPoint(1, 4)));
            Assert.IsFalse(p.Contains(new IntPoint(3, 1)));
            Assert.IsFalse(p.Contains(new IntPoint(1, 7)));
            Assert.IsFalse(p.Contains(new IntPoint(5, 1)));
            Assert.IsFalse(p.Contains(new IntPoint(8, 9)));
        }
        [Test]
        public void Equality()
        {
            Assert.IsTrue(new IntPoint(2, 3) == new IntPoint(2, 3));
            Assert.IsFalse(new IntPoint(2, 3) != new IntPoint(2, 3));
            Assert.IsFalse(new IntPoint(2, 3) == new IntPoint(0, 3));
            Assert.IsTrue(new IntPoint(2, 3) != new IntPoint(0, 3));
            Assert.IsFalse(new IntPoint(2, 3) == new IntPoint(2, 0));
            Assert.IsTrue(new IntPoint(2, 3) != new IntPoint(2, 0));
        }
        [Test]
        public void Equals()
        {
            Assert.IsTrue(new IntPoint(2, 3).Equals(new IntPoint(2, 3)));
            Assert.IsFalse(new IntPoint(2, 3).Equals(new IntPoint(0, 3)));
            Assert.IsFalse(new IntPoint(2, 3).Equals(new IntPoint(2, 0)));
            Assert.IsFalse(new IntPoint(2, 3).Equals(null));
            Assert.IsFalse(new IntPoint(2, 3).Equals(1));
        }
        [Test]
        public void HashCode()
        {
            Assert.AreEqual(new IntPoint(2, 3).GetHashCode(), new IntPoint(2, 3).GetHashCode());
            Assert.AreNotEqual(new IntPoint(2, 3).GetHashCode(), new IntPoint(-2, 3).GetHashCode());
            Assert.AreNotEqual(new IntPoint(2, 3).GetHashCode(), new IntPoint(2, -3).GetHashCode());
        }
        [Test]
        public void EdgeNeighbors()
        {
            var s = new HashSet<IntPoint>();
            foreach (var n in IntPoint.EdgeNeighbors)
            {
                s.Add(n);
                Assert.AreEqual(1, n.LengthSq);
            }
            Assert.AreEqual(4, s.Count);
        }
        [Test]
        public void CornerNeighbors()
        {
            var s = new HashSet<IntPoint>();
            foreach (var n in IntPoint.CornerNeighbors)
            {
                s.Add(n);
                Assert.IsTrue(n.LengthSq == 1 || n.LengthSq == 2);
            }
            Assert.AreEqual(8, s.Count);
        }
        [Test]
        public void Iterate()
        {
            var l = new List<IntPoint>();
            foreach (var p in new IntPoint(2, 3).Iterate())
                l.Add(p);
            Assert.AreEqual(new[] { new IntPoint(0, 0), new IntPoint(1, 0), new IntPoint(0, 1), new IntPoint(1, 1), new IntPoint(0, 2), new IntPoint(1, 2) }, l);
            foreach (var p in new IntPoint(0, 3).Iterate())
                Assert.Fail();
            foreach (var p in new IntPoint(3, 0).Iterate())
                Assert.Fail();
            foreach (var p in new IntPoint(-1, 3).Iterate())
                Assert.Fail();
            foreach (var p in new IntPoint(3, -1).Iterate())
                Assert.Fail();
        }
        [Test]
        public void lineTo()
        {
            CheckLineTo(2, 3, 2, 3, 2, 3);
            CheckLineTo(2, 3, 1, 4, 2, 3, 1, 4);
            CheckLineTo(2, 3, -1, 3, 2, 3, 1, 3, 0, 3, -1, 3);
            CheckLineTo(-1, 2, 0, -1, -1, 2, -1, 1, 0, 0, 0, -1);
            CheckLineTo(1, 1, 3, 7, 1, 1, 1, 2, 2, 3, 2, 4, 2, 5, 3, 6, 3, 7);
            CheckLineTo(1, 3, 6, 1, 1, 3, 2, 3, 3, 2, 4, 2, 5, 1, 6, 1);
        }
        void CheckLineTo(int x1, int y1, int x2, int y2, params int[] p)
        {
            var l = new IntPoint[p.Length / 2];
            for (int i = 0; i < l.Length; ++i)
                l[i] = new IntPoint(p[2 * i], p[2 * i + 1]);
            Assert.AreEqual(l, new IntPoint(x1, y1).LineTo(new IntPoint(x2, y2)));
        }
        [Test]
        public void ToStringReadable()
        {
            Assert.AreEqual("[2,3]", new IntPoint(2, 3).ToString());
        }
    }
}
