// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class ReversedListTest
    {
        List<int> o;
        ReversedList<int> r;

        [SetUp]
        public void SetUp()
        {
            o = new List<int>();
            r = new ReversedList<int>(o);
            for (int i = 0; i < 5; ++i)
                o.Add(i + 1);
        }
        [Test]
        public void Add()
        {
            r.Add(10);
            Assert.AreEqual(new[] { 10, 1, 2, 3, 4, 5 }, o);
        }
        [Test]
        public void Insert()
        {
            r.Insert(1, 10);
            r.Insert(6, 20);
            r.Insert(0, 30);
            Assert.AreEqual(new[] { 20, 1, 2, 3, 4, 10, 5, 30 }, o);
        }
        [Test]
        public void InsertBounds() => Assert.Throws<ArgumentOutOfRangeException>(() => r.Insert(6, 10));
        [Test]
        public void Clear()
        {
            r.Clear();
            Assert.AreEqual(0, o.Count);
        }
        [Test]
        public void Contains()
        {
            Assert.IsTrue(r.Contains(3));
            Assert.IsFalse(r.Contains(10));
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(4, r[1]);
            Assert.AreEqual(2, r[3]);
            int discarded;
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = r[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = r[5]);
        }
        [Test]
        public void IndexOf()
        {
            r.Add(4);
            Assert.AreEqual(1, r.IndexOf(4));
            Assert.AreEqual(-1, r.IndexOf(10));
        }
        [Test]
        public void Enumerable()
        {
            var c = new List<int>();
            foreach (var n in r)
                c.Add(n);
            Assert.AreEqual(new[] { 5, 4, 3, 2, 1 }, c);
        }
        [Test]
        public void RemoveAt()
        {
            r.RemoveAt(1);
            Assert.AreEqual(new[] { 5, 3, 2, 1 }, r);
        }
        [Test]
        public void RemoveAtBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => r.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => r.RemoveAt(5));
        }
        [Test]
        public void Remove()
        {
            r.Add(4);
            Assert.IsTrue(r.Remove(4));
            Assert.IsFalse(r.Remove(10));
            Assert.AreEqual(new[] { 5, 3, 2, 1, 4 }, r);
        }
        [Test]
        public void Set()
        {
            r[1] = 10;
            Assert.AreEqual(new[] { 5, 10, 3, 2, 1 }, r);
            Assert.Throws<ArgumentOutOfRangeException>(() => r[-1] = 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => r[5] = 10);
        }
        [Test]
        public void Count() => Assert.AreEqual(5, r.Count);
    }
}
