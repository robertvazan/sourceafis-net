// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS;
using Dahomey.Cbor;

namespace SourceAFIS.Tests
{
	public class FingerprintTemplateTest
	{
		public static FingerprintTemplate ProbeGray() { return new FingerprintTemplate(FingerprintImageTest.ProbeGray()); }
		public static FingerprintTemplate MatchingGray() { return new FingerprintTemplate(FingerprintImageTest.MatchingGray()); }
		public static FingerprintTemplate NonmatchingGray() { return new FingerprintTemplate(FingerprintImageTest.NonmatchingGray()); }

		[Test]
		public void Serialize()
		{
			byte[] serialized = ProbeGray().ToByteArray();
			string json = Cbor.ToJson(serialized);
			Console.WriteLine(json);
			Assert.Fail();
		}
	}
}
