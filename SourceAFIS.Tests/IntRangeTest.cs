// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
	public class IntRangeTest
	{
		[Test]
		public void Constructor()
		{
			var r = new IntRange(3, 10);
			Assert.AreEqual(3, r.Start);
			Assert.AreEqual(10, r.End);
		}
		[Test]
		public void Length()
		{
			Assert.AreEqual(7, new IntRange(3, 10).Length);
		}
	}
}
