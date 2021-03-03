// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using NUnit.Framework;
using SourceAFIS;
using Dahomey.Cbor;

namespace SourceAFIS.Tests
{
	public class SerializationUtilsTest
	{
		class TestClass
		{
			public int PublicField = 123;
			int PrivateField = 456;
			public int PublicProperty { get; set; } = 999;
			int PrivateProperty { get; set; } = 999;
		}

		[Test]
		public void Serialize()
		{
			byte[] cbor = SerializationUtils.Serialize(new TestClass());
			Assert.AreEqual("{\"publicField\":123,\"privateField\":456}", Cbor.ToJson(cbor));
		}
	}
}
