// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class CircularArrayTest
    {
        CircularArray<Object> A;

        [SetUp]
        public void SetUp()
        {
            A = new CircularArray<Object>(16);
            A.Insert(0, 16);
            A.Remove(0, 10);
            A.Insert(A.Size, 4);
            for (int i = 0; i < A.Size; ++i)
                A[i] = i + 1;
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
            A.ValidateItemIndex(0);
            A.ValidateItemIndex(5);
            A.ValidateItemIndex(9);
            Assert.Throws<ArgumentOutOfRangeException>(() => A.ValidateItemIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => A.ValidateItemIndex(10));
        }
        [Test]
        public void ValidateCursorIndex()
        {
            A.ValidateCursorIndex(0);
            A.ValidateCursorIndex(4);
            A.ValidateCursorIndex(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => A.ValidateCursorIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => A.ValidateCursorIndex(11));
        }
        [Test]
        public void Location()
        {
            Assert.AreEqual(10, A.Location(0));
            Assert.AreEqual(14, A.Location(4));
            Assert.AreEqual(15, A.Location(5));
            Assert.AreEqual(0, A.Location(6));
            Assert.AreEqual(2, A.Location(8));
        }
        [Test]
        public void Enlarge()
        {
            A.Enlarge();
            Assert.AreEqual(0, A.Head);
            Assert.AreEqual(10, A.Size);
            Assert.AreEqual(32, A.Array.Length);
            for (int i = 0; i < 10; ++i)
                Assert.AreEqual(i + 1, A[i]);
        }
        [Test]
        public void Get()
        {
            Assert.AreEqual(1, A[0]);
            Assert.AreEqual(6, A[5]);
            Assert.AreEqual(10, A[9]);
            Object discarded;
            Assert.Throws<ArgumentOutOfRangeException>(() => discarded = A[A.Size]);
        }
        [Test]
        public void Set()
        {
            A[4] = 100;
            A[8] = 200;
            Assert.AreEqual(100, A[4]);
            Assert.AreEqual(200, A[8]);
            Assert.AreEqual(100, A.Array[14]);
            Assert.AreEqual(200, A.Array[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => A[A.Size] = 100);
        }
        [Test]
        public void MoveLeft()
        {
            A.Move(4, 2, 5);
            Assert.AreEqual(new Object[] { 9, 8, 9, 10, null, null, null, null, null, null, 1, 2, 5, 6, 7, 8 }, A.Array);
        }
        [Test]
        public void MoveRight()
        {
            A.Move(2, 4, 5);
            Assert.AreEqual(new Object[] { 5, 6, 7, 10, null, null, null, null, null, null, 1, 2, 3, 4, 3, 4 }, A.Array);
        }
        [Test]
        public void InsertEnd()
        {
            A.Insert(A.Size, 3);
            Assert.AreEqual(10, A.Head);
            Assert.AreEqual(13, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void InsertRight()
        {
            A.Insert(8, 3);
            Assert.AreEqual(10, A.Head);
            Assert.AreEqual(13, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, null, null, null, 9, 10, null, null, null, 1, 2, 3, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void InsertLeft()
        {
            A.Insert(2, 3);
            Assert.AreEqual(7, A.Head);
            Assert.AreEqual(13, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, 1, 2, null, null, null, 3, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void InsertFront()
        {
            A.Insert(0, 3);
            Assert.AreEqual(7, A.Head);
            Assert.AreEqual(13, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void InsertBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => A.Insert(-1, 1));
        }
        [Test]
        public void InsertNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => A.Insert(5, -1));
        }
        [Test]
        public void InsertEnlarge()
        {
            A.Insert(A.Size, 200);
            Assert.AreEqual(0, A.Head);
            Assert.AreEqual(210, A.Size);
            Assert.AreEqual(256, A.Array.Length);
        }
        [Test]
        public void RemoveEnd()
        {
            A.Remove(7, 3);
            Assert.AreEqual(10, A.Head);
            Assert.AreEqual(7, A.Size);
            Assert.AreEqual(new Object[] { 7, null, null, null, null, null, null, null, null, null, 1, 2, 3, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void RemoveRight()
        {
            A.Remove(4, 3);
            Assert.AreEqual(10, A.Head);
            Assert.AreEqual(7, A.Size);
            Assert.AreEqual(new Object[] { 10, null, null, null, null, null, null, null, null, null, 1, 2, 3, 4, 8, 9 }, A.Array);
        }
        [Test]
        public void RemoveLeft()
        {
            A.Remove(2, 3);
            Assert.AreEqual(13, A.Head);
            Assert.AreEqual(7, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, null, null, null, 1, 2, 6 }, A.Array);
        }
        [Test]
        public void RemoveFront()
        {
            A.Remove(0, 3);
            Assert.AreEqual(13, A.Head);
            Assert.AreEqual(7, A.Size);
            Assert.AreEqual(new Object[] { 7, 8, 9, 10, null, null, null, null, null, null, null, null, null, 4, 5, 6 }, A.Array);
        }
        [Test]
        public void RemoveBoundsLeft()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => A.Remove(-1, 3));
        }
        [Test]
        public void RemoveBoundsRight()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => A.Remove(8, 3));
        }
        [Test]
        public void RemoveNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => A.Remove(5, -1));
        }
    }
}
