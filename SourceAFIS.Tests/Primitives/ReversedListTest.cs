// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class ReversedListTest
    {
        List<int> O;
        ReversedList<int> R;

        [SetUp]
        public void SetUp()
        {
            O = new List<int>();
            R = new ReversedList<int>(O);
            for (int i = 0; i < 5; ++i)
                O.Add(i + 1);
        }
        [Test]
        public void Add()
        {
            R.Add(10);
            Assert.AreEqual(new[] { 10, 1, 2, 3, 4, 5 }, O);
        }
        [Test]
        public void Insert()
        {
            R.Insert(1, 10);
            R.Insert(6, 20);
            R.Insert(0, 30);
            Assert.AreEqual(new[] { 20, 1, 2, 3, 4, 10, 5, 30 }, O);
        }
        [Test]
        public void InsertBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => R.Insert(6, 10));
        }
        [Test]
        public void Clear()
        {
            R.Clear();
            Assert.AreEqual(0, O.Count);
        }
        [Test]
        public void Contains()
        {
            Assert.IsTrue(R.Contains(3));
            Assert.IsFalse(R.Contains(10));
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(4, R[1]);
            Assert.AreEqual(2, R[3]);
            int discarded;
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = R[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = R[5]);
        }
        [Test]
        public void IndexOf()
        {
            R.Add(4);
            Assert.AreEqual(1, R.IndexOf(4));
            Assert.AreEqual(-1, R.IndexOf(10));
        }
        [Test]
        public void Enumerable()
        {
            var c = new List<int>();
            foreach (var n in R)
                c.Add(n);
            Assert.AreEqual(new[] { 5, 4, 3, 2, 1 }, c);
        }
        [Test]
        public void RemoveAt()
        {
            R.RemoveAt(1);
            Assert.AreEqual(new[] { 5, 3, 2, 1 }, R);
        }
        [Test]
        public void RemoveAtBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => R.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => R.RemoveAt(5));
        }
        [Test]
        public void Remove()
        {
            R.Add(4);
            Assert.IsTrue(R.Remove(4));
            Assert.IsFalse(R.Remove(10));
            Assert.AreEqual(new[] { 5, 3, 2, 1, 4 }, R);
        }
        [Test]
        public void Set()
        {
            R[1] = 10;
            Assert.AreEqual(new[] { 5, 10, 3, 2, 1 }, R);
            Assert.Throws<ArgumentOutOfRangeException>(() => R[-1] = 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => R[5] = 10);
        }
        [Test]
        public void Count()
        {
            Assert.AreEqual(5, R.Count);
        }
    }
}
