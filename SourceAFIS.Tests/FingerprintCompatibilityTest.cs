// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;

namespace SourceAFIS
{
    public class FingerprintCompatibilityTest
    {
        [Test]
        public void Version() => Assert.That(FingerprintCompatibility.Version, Does.Match("^\\d+\\.\\d+\\.\\d+$"));
    }
}
