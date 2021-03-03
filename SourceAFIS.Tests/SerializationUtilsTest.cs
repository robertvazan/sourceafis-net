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

			readonly int ReadonlyField = 999;
			const int ConstField = 999;
			public int PublicProperty { get; set; } = 999;
			int PrivateProperty { get; set; } = 999;

			public int GetPrivate() { return PrivateField; }
			public void SetPrivate(int value) { PrivateField = value; }
		}

		[Test]
		public void Serialize()
		{
			byte[] cbor = SerializationUtils.Serialize(new TestClass());
			Assert.AreEqual("{\"publicField\":123,\"privateField\":456}", Cbor.ToJson(cbor));
		}
		[Test]
		public void Deserialize()
		{
			var data = new TestClass();
			data.PublicField = 777;
			data.SetPrivate(888);
			data = SerializationUtils.Deserialize<TestClass>(SerializationUtils.Serialize(data));
			Assert.AreEqual(777, data.PublicField);
			Assert.AreEqual(888, data.GetPrivate());
		}
	}
}
