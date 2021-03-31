// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class FingerprintCompatibilityTest
    {
        [Test]
        public void Version()
        {
            Assert.That(FingerprintCompatibility.Version, Does.Match("^\\d+\\.\\d+\\.\\d+$"));
        }
    }
}
