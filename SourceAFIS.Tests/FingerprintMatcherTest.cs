// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;

namespace SourceAFIS.Tests
{
    public class FingerprintMatcherTest
    {
        void Matching(FingerprintTemplate probe, FingerprintTemplate candidate)
        {
            double score = new FingerprintMatcher(probe)
                .Match(candidate);
            Assert.Greater(score, 40);
        }
        void Nonmatching(FingerprintTemplate probe, FingerprintTemplate candidate)
        {
            double score = new FingerprintMatcher(probe)
                .Match(candidate);
            Assert.Less(score, 20);
        }
        [Test] public void MatchingPair() { Matching(FingerprintTemplateTest.Probe(), FingerprintTemplateTest.Matching()); }
        [Test] public void NonmatchingPair() { Nonmatching(FingerprintTemplateTest.Probe(), FingerprintTemplateTest.Nonmatching()); }
        [Test] public void MatchingGray() { Matching(FingerprintTemplateTest.ProbeGray(), FingerprintTemplateTest.MatchingGray()); }
        [Test] public void NonmatchingGray() { Nonmatching(FingerprintTemplateTest.ProbeGray(), FingerprintTemplateTest.NonmatchingGray()); }
    }
}
