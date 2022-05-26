// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class CircularListTest
    {
        CircularList<int> l;

        [SetUp]
        public void SetUp()
        {
            l = new CircularList<int>();
            for (int i = 0; i < 5; ++i)
                l.Add(i + 1);
        }
        [Test]
        public void Add()
        {
            l.Add(100);
            Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 100 }, l);
        }
        [Test]
        public void Insert()
        {
            l.Insert(3, 100);
            l.Insert(6, 200);
            l.Insert(0, 300);
            Assert.AreEqual(new[] { 300, 1, 2, 3, 100, 4, 5, 200 }, l);
        }
        [Test]
        public void InsertBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => l.Insert(6, 10));
        }
        [Test]
        public void Clear()
        {
            l.Clear();
            Assert.AreEqual(0, l.Count);
        }
        [Test]
        public void Contains()
        {
            Assert.IsTrue(l.Contains(3));
            Assert.IsFalse(l.Contains(10));
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(2, l[1]);
            Assert.AreEqual(4, l[3]);
            int discarded;
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = l[5]);
        }
        [Test]
        public void IndexOf()
        {
            l.Add(3);
            Assert.AreEqual(2, l.IndexOf(3));
            Assert.AreEqual(-1, l.IndexOf(10));
        }
        [Test]
        public void Enumerator()
        {
            var c = new List<int>();
            foreach (var n in l)
                c.Add(n);
            Assert.AreEqual(new[] { 1, 2, 3, 4, 5 }, c);
        }
        [Test]
        public void RemoveAt()
        {
            l.RemoveAt(2);
            Assert.AreEqual(new[] { 1, 2, 4, 5 }, l);
            Assert.Throws<ArgumentOutOfRangeException>(() => l.RemoveAt(5));
        }
        [Test]
        public void Remove()
        {
            Assert.IsTrue(l.Remove(2));
            Assert.IsFalse(l.Remove(10));
            Assert.AreEqual(new[] { 1, 3, 4, 5 }, l);
        }
        [Test]
        public void Set()
        {
            l[2] = 10;
            Assert.AreEqual(new[] { 1, 2, 10, 4, 5 }, l);
            Assert.Throws<ArgumentOutOfRangeException>(() => l[5] = 10);
        }
        [Test]
        public void Count()
        {
            Assert.AreEqual(5, l.Count);
        }
    }
}
