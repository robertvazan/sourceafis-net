// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS;

namespace SourceAFIS.Tests
{
	public class FingerprintImageTest
	{
		public static FingerprintImage ProbeGray() { return FingerprintImage.Grayscale(332, 533, TestResources.ProbeGray()); }
		public static FingerprintImage MatchingGray() { return FingerprintImage.Grayscale(320, 407, TestResources.MatchingGray()); }
		public static FingerprintImage NonmatchingGray() { return FingerprintImage.Grayscale(333, 435, TestResources.NonmatchingGray()); }
		[Test]
		public void DecodeGray()
		{
			new FingerprintTemplate(ProbeGray());
		}
	}
}
