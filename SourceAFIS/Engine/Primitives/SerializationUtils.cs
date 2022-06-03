// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dahomey.Cbor;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using Dahomey.Cbor.Util;

namespace SourceAFIS.Engine.Primitives
{
    static class SerializationUtils
    {
        // Dahomey.Cbor requires default constructor even if we perform only serialization.
        // We will give it fake object factory to keep it happy.
        class NoDefaultConstructor : ICreatorMapping
        {
            public IReadOnlyCollection<RawString> MemberNames => null;

            public void Initialize() { }
            public object CreateInstance(Dictionary<RawString, object> values) => throw new NotImplementedException();
        }

        // Conventions consistent with Java.
        class ConsistentConvention : IObjectMappingConvention
        {
            static void CollectFields<T>(ObjectMapping<T> mapping, Type type)
            {
                // Reflection will not give us private fields of base classes, so walk base classes explicitly.
                var parent = type.BaseType;
                if (parent != null)
                    CollectFields(mapping, parent);
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    if (field.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
                        mapping.MapMember(field, field.FieldType);
            }
            public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> mapping)
            {
                // Java field naming convention.
                mapping.SetNamingConvention(new CamelCaseNamingConvention());
                // Do not serialize properties, only fields. Include both public and private fields.
                CollectFields(mapping, mapping.ObjectType);
                // Allow serialization of objects without default constructor. We don't need deserialization.
                var constructors = mapping.ObjectType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (!constructors.Any(c => c.GetParameters().Length == 0))
                    mapping.SetCreatorMapping(new NoDefaultConstructor());
            }
        }

        class ConsistentConventionProvider : IObjectMappingConventionProvider
        {
            public IObjectMappingConvention GetConvention(Type type) => new ConsistentConvention();
        }

        static readonly CborOptions Options = new CborOptions();

        static SerializationUtils()
        {
            Options.Registry.ObjectMappingConventionRegistry.RegisterProvider(new ConsistentConventionProvider());
        }

        public static byte[] Serialize(object data)
        {
            using (var buffer = new ByteBufferWriter())
            {
                Cbor.Serialize(data, data.GetType(), buffer, Options);
                return buffer.WrittenSpan.ToArray();
            }
        }
        public static T Deserialize<T>(byte[] bytes) => Cbor.Deserialize<T>(bytes, Options);
    }
}
