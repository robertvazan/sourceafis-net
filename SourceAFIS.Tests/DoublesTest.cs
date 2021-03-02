// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS;

namespace SourceAFIS.Tests
{
	class DoublesTest
	{
		[Test]
		public void RoundToInt()
		{
			Assert.AreEqual(7, Doubles.RoundToInt(7));
			Assert.AreEqual(5, Doubles.RoundToInt(5.4));
			Assert.AreEqual(9, Doubles.RoundToInt(8.6));
			Assert.AreEqual(4, Doubles.RoundToInt(3.5));
			Assert.AreEqual(5, Doubles.RoundToInt(4.5));
			Assert.AreEqual(-6, Doubles.RoundToInt(-6.4));
			Assert.AreEqual(-8, Doubles.RoundToInt(-7.6));
			Assert.AreEqual(-3, Doubles.RoundToInt(-3.5));
			Assert.AreEqual(-4, Doubles.RoundToInt(-4.5));
		}
		[Test]
		public void Sq()
		{
			Assert.AreEqual(6.25, Doubles.Sq(2.5), 0.001);
			Assert.AreEqual(6.25, Doubles.Sq(-2.5), 0.001);
		}
		[Test]
		public void Interpolate1D()
		{
			Assert.AreEqual(5, Doubles.Interpolate(3, 7, 0.5), 0.001);
			Assert.AreEqual(3, Doubles.Interpolate(3, 7, 0), 0.001);
			Assert.AreEqual(7, Doubles.Interpolate(3, 7, 1), 0.001);
			Assert.AreEqual(6, Doubles.Interpolate(7, 3, 0.25), 0.001);
			Assert.AreEqual(11, Doubles.Interpolate(7, 3, -1), 0.001);
			Assert.AreEqual(9, Doubles.Interpolate(3, 7, 1.5), 0.001);
		}
		[Test]
		public void Interpolate2D()
		{
			Assert.AreEqual(2, Doubles.Interpolate(3, 7, 2, 4, 0, 0), 0.001);
			Assert.AreEqual(4, Doubles.Interpolate(3, 7, 2, 4, 1, 0), 0.001);
			Assert.AreEqual(3, Doubles.Interpolate(3, 7, 2, 4, 0, 1), 0.001);
			Assert.AreEqual(7, Doubles.Interpolate(3, 7, 2, 4, 1, 1), 0.001);
			Assert.AreEqual(2.5, Doubles.Interpolate(3, 7, 2, 4, 0, 0.5), 0.001);
			Assert.AreEqual(5.5, Doubles.Interpolate(3, 7, 2, 4, 1, 0.5), 0.001);
			Assert.AreEqual(3, Doubles.Interpolate(3, 7, 2, 4, 0.5, 0), 0.001);
			Assert.AreEqual(5, Doubles.Interpolate(3, 7, 2, 4, 0.5, 1), 0.001);
			Assert.AreEqual(4, Doubles.Interpolate(3, 7, 2, 4, 0.5, 0.5), 0.001);
		}
		[Test]
		public void InterpolateExponential()
		{
			Assert.AreEqual(3, Doubles.InterpolateExponential(3, 10, 0), 0.001);
			Assert.AreEqual(10, Doubles.InterpolateExponential(3, 10, 1), 0.001);
			Assert.AreEqual(3, Doubles.InterpolateExponential(1, 9, 0.5), 0.001);
			Assert.AreEqual(27, Doubles.InterpolateExponential(1, 9, 1.5), 0.001);
			Assert.AreEqual(1 / 3.0, Doubles.InterpolateExponential(1, 9, -0.5), 0.001);
		}
	}
}
