// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS;

namespace SourceAFIS.Tests
{
	public class FingerprintImageTest
	{
		[Test]
		public void DecodeGray()
		{
			TestResources.ProbeGray();
		}
	}
}
