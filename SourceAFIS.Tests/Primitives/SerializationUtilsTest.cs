// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using NUnit.Framework;
using Dahomey.Cbor;

namespace SourceAFIS.Primitives
{
    public class SerializationUtilsTest
    {
        class BaseClass
        {
            public int BaseField = 1;
        }
        class TestClass : BaseClass
        {
            public int PublicField = 2;
            int PrivateField = 3;

            const int ConstField = 9;
            public int PublicProperty { get; set; } = 9;
            int PrivateProperty { get; set; } = 9;

            public int GetPrivate() => PrivateField;
            public void SetPrivate(int value) => PrivateField = value;
        }
        class ImmutableClass : TestClass
        {
            public readonly int ReadonlyField;

            public ImmutableClass(int value) => ReadonlyField = value;
        }

        [Test]
        public void Serialize()
        {
            byte[] cbor = SerializationUtils.Serialize(new ImmutableClass(4));
            Assert.AreEqual("{\"baseField\":1,\"publicField\":2,\"privateField\":3,\"readonlyField\":4}", Cbor.ToJson(cbor));
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
