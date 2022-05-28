// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Primitives
{
    public class CircularArrayTest
    {
        CircularArray<Object> a;

        [SetUp]
        public void SetUp()
        {
            a = new CircularArray<Object>(16);
            a.Insert(0, 16);
            a.Remove(0, 10);
            a.Insert(a.Size, 4);
            for (int i = 0; i < a.Size; ++i)
                a[i] = i + 1;
        }
        [Test]
        public void Constructor()
        {
            var a = new CircularArray<int>(13);
            Assert.AreEqual(13, a.Array.Length);
            Assert.AreEqual(0, a.Size);
            Assert.AreEqual(0, a.Head);
        }
        [Test]
        public void ValidateItemIndex()
        {
            a.ValidateItemIndex(0);
            a.ValidateItemIndex(5);
            a.ValidateItemIndex(9);
            Assert.Throws<ArgumentOutOfRangeException>(() => a.ValidateItemIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => a.ValidateItemIndex(10));
        }
        [Test]
        public void ValidateCursorIndex()
        {
            a.ValidateCursorIndex(0);
            a.ValidateCursorIndex(4);
            a.ValidateCursorIndex(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => a.ValidateCursorIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => a.ValidateCursorIndex(11));
        }
        [Test]
        public void Location()
        {
            Assert.AreEqual(10, a.Location(0));
            Assert.AreEqual(14, a.Location(4));
            Assert.AreEqual(15, a.Location(5));
            Assert.AreEqual(0, a.Location(6));
            Assert.AreEqual(2, a.Location(8));
        }
        [Test]
        public void Enlarge()
        {
            a.Enlarge();
            Assert.AreEqual(0, a.Head);
            Assert.AreEqual(10, a.Size);
            Assert.AreEqual(32, a.Array.Length);
            for (int i = 0; i < 10; ++i)
                Assert.AreEqual(i + 1, a[i]);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(1, a[0]);
            Assert.AreEqual(6, a[5]);
            Assert.AreEqual(10, a[9]);
            Object discarded;
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = a[a.Size]);
        }
        [Test]
        public void Set()
        {
            a[4] = 100;
            a[8] = 200;
            Assert.AreEqual(100, a[4]);
            Assert.AreEqual(200, a[8]);
            Assert.AreEqual(100, a.Array[14]);
            Assert.AreEqual(200, a.Array[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => a[a.Size] = 100);
        }
        [Test]
        public void MoveLeft()
        {
            a.Move(4, 2, 5);
            Assert.AreEqual(new Object[] { 9, 8, 9, 10, null, null, null, null, null, null, 1, 2, 5, 6, 7, 8 }, a.Array);
        }
        [Test]
        public void MoveRight()
        {
            a.Move(2, 4, 5);
            Assert.AreEqual(new Object[] { 5, 6, 7, 10, null, null, null, null, null, null, 1, 2, 3, 4, 3, 4 }, a.Array);
        }
        [Test]
        public void InsertEnd()
        {
            a.Insert(a.Size, 3);
            Assert.AreEqual(10, a.Head);
            Assert.AreEqual(13, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void InsertRight()
        {
            a.Insert(8, 3);
            Assert.AreEqual(10, a.Head);
            Assert.AreEqual(13, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, null, null, null, 9, 10, null, null, null, 1, 2, 3, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void InsertLeft()
        {
            a.Insert(2, 3);
            Assert.AreEqual(7, a.Head);
            Assert.AreEqual(13, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, 1, 2, null, null, null, 3, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void InsertFront()
        {
            a.Insert(0, 3);
            Assert.AreEqual(7, a.Head);
            Assert.AreEqual(13, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void InsertBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => a.Insert(-1, 1));
        }
        [Test]
        public void InsertNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => a.Insert(5, -1));
        }
        [Test]
        public void InsertEnlarge()
        {
            a.Insert(a.Size, 200);
            Assert.AreEqual(0, a.Head);
            Assert.AreEqual(210, a.Size);
            Assert.AreEqual(256, a.Array.Length);
        }
        [Test]
        public void RemoveEnd()
        {
            a.Remove(7, 3);
            Assert.AreEqual(10, a.Head);
            Assert.AreEqual(7, a.Size);
            Assert.AreEqual(new Object[] { 7, null, null, null, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void RemoveRight()
        {
            a.Remove(4, 3);
            Assert.AreEqual(10, a.Head);
            Assert.AreEqual(7, a.Size);
            Assert.AreEqual(new Object[] { 10, null, null, null, null, null, null, null, null, null, 1, 2, 3, 4, 8, 9 }, a.Array);
        }
        [Test]
        public void RemoveLeft()
        {
            a.Remove(2, 3);
            Assert.AreEqual(13, a.Head);
            Assert.AreEqual(7, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, null, null, null, 1, 2, 6 }, a.Array);
        }
        [Test]
        public void RemoveFront()
        {
            a.Remove(0, 3);
            Assert.AreEqual(13, a.Head);
            Assert.AreEqual(7, a.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, null, null, null, 4, 5, 6 }, a.Array);
        }
        [Test]
        public void RemoveBoundsLeft() => Assert.Throws<ArgumentOutOfRangeException>(() => a.Remove(-1, 3));
        [Test]
        public void RemoveBoundsRight() => Assert.Throws<ArgumentOutOfRangeException>(() => a.Remove(8, 3));
        [Test]
        public void RemoveNegative() => Assert.Throws<ArgumentOutOfRangeException>(() => a.Remove(5, -1));
    }
}
