// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class IntegersTest
    {
        [Test]
        public void Sq()
        {
            Assert.AreEqual(9, Integers.Sq(3));
            Assert.AreEqual(9, Integers.Sq(-3));
        }
        [Test]
        public void RoundUpDiv()
        {
            Assert.AreEqual(3, Integers.RoundUpDiv(9, 3));
            Assert.AreEqual(3, Integers.RoundUpDiv(8, 3));
            Assert.AreEqual(3, Integers.RoundUpDiv(7, 3));
            Assert.AreEqual(2, Integers.RoundUpDiv(6, 3));
            Assert.AreEqual(5, Integers.RoundUpDiv(20, 4));
            Assert.AreEqual(5, Integers.RoundUpDiv(19, 4));
            Assert.AreEqual(5, Integers.RoundUpDiv(18, 4));
            Assert.AreEqual(5, Integers.RoundUpDiv(17, 4));
            Assert.AreEqual(4, Integers.RoundUpDiv(16, 4));
        }
        [Test]
        public void PopulationCount()
        {
            Assert.AreEqual(0, Integers.PopulationCount(0));
            Assert.AreEqual(1, Integers.PopulationCount(0x80));
            Assert.AreEqual(2, Integers.PopulationCount(0x40008000));
            Assert.AreEqual(3, Integers.PopulationCount(0x40208000));
        }
        [Test]
        public void LeadingZeros()
        {
            Assert.AreEqual(32, Integers.LeadingZeros(0));
            Assert.AreEqual(24, Integers.LeadingZeros(0x80));
            Assert.AreEqual(1, Integers.LeadingZeros(0x40008000));
        }
    }
}
