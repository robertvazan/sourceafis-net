// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
	public class IntRectTest
	{
		[Test]
		public void Constructor()
		{
			var r = new IntRect(2, 3, 10, 20);
			Assert.AreEqual(2, r.X);
			Assert.AreEqual(3, r.Y);
			Assert.AreEqual(10, r.Width);
			Assert.AreEqual(20, r.Height);
		}
		[Test]
		public void ConstructorFromPoint()
		{
			var r = new IntRect(new IntPoint(2, 3));
			Assert.AreEqual(0, r.X);
			Assert.AreEqual(0, r.Y);
			Assert.AreEqual(2, r.Width);
			Assert.AreEqual(3, r.Height);
		}
		[Test]
		public void Left()
		{
			Assert.AreEqual(2, new IntRect(2, 3, 4, 5).Left);
		}
		[Test]
		public void Right()
		{
			Assert.AreEqual(6, new IntRect(2, 3, 4, 5).Right);
		}
		[Test]
		public void Bottom()
		{
			Assert.AreEqual(3, new IntRect(2, 3, 4, 5).Top);
		}
		[Test]
		public void Top()
		{
			Assert.AreEqual(8, new IntRect(2, 3, 4, 5).Bottom);
		}
		[Test]
		public void Area()
		{
			Assert.AreEqual(20, new IntRect(2, 3, 4, 5).Area);
		}
		[Test]
		public void BetweenCoordinates()
		{
			Assert.AreEqual(new IntRect(2, 3, 4, 5), IntRect.Between(2, 3, 6, 8));
		}
		[Test]
		public void BetweenPoints()
		{
			Assert.AreEqual(new IntRect(2, 3, 4, 5), IntRect.Between(new IntPoint(2, 3), new IntPoint(6, 8)));
		}
		[Test]
		public void AroundCoordinates()
		{
			Assert.AreEqual(new IntRect(2, 3, 5, 5), IntRect.Around(4, 5, 2));
		}
		[Test]
		public void AroundPoint()
		{
			Assert.AreEqual(new IntRect(2, 3, 5, 5), IntRect.Around(new IntPoint(4, 5), 2));
		}
		[Test]
		public void Center()
		{
			Assert.AreEqual(new IntPoint(4, 5), new IntRect(2, 3, 4, 4).Center);
			Assert.AreEqual(new IntPoint(4, 5), new IntRect(2, 3, 5, 5).Center);
			Assert.AreEqual(new IntPoint(2, 3), new IntRect(2, 3, 0, 0).Center);
		}
		[Test]
		public void Move()
		{
			Assert.AreEqual(new IntRect(12, 23, 4, 5), new IntRect(2, 3, 4, 5).Move(new IntPoint(10, 20)));
		}
		[Test]
		public void Intersect()
		{
			Assert.AreEqual(new IntRect(58, 30, 2, 5), new IntRect(20, 30, 40, 50).Intersect(new IntRect(58, 27, 7, 8)));
			Assert.AreEqual(new IntRect(20, 77, 5, 3), new IntRect(20, 30, 40, 50).Intersect(new IntRect(18, 77, 7, 8)));
			Assert.AreEqual(new IntRect(30, 40, 20, 30), new IntRect(20, 30, 40, 50).Intersect(new IntRect(30, 40, 20, 30)));
		}
		[Test]
		public void Iterate()
		{
			var l = new List<IntPoint>();
			foreach (var p in new IntRect(4, 5, 2, 3).Iterate())
				l.Add(p);
			Assert.AreEqual(new[] { new IntPoint(4, 5), new IntPoint(5, 5), new IntPoint(4, 6), new IntPoint(5, 6), new IntPoint(4, 7), new IntPoint(5, 7) }, l);
			foreach (var p in new IntRect(2, 3, 0, 3).Iterate())
				Assert.Fail();
			foreach (var p in new IntRect(2, 3, 3, 0).Iterate())
				Assert.Fail();
			foreach (var p in new IntRect(2, 3, -1, 3).Iterate())
				Assert.Fail();
			foreach (var p in new IntRect(2, 3, 3, -1).Iterate())
				Assert.Fail();
		}
		[Test]
		public void ToStringReadable()
		{
			Assert.AreEqual("10x20 @ [2,3]", new IntRect(2, 3, 10, 20).ToString());
		}
	}
}
