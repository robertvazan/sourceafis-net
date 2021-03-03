// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using Dahomey.Cbor;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using Dahomey.Cbor.Util;

namespace SourceAFIS
{
	static class SerializationUtils
	{
		// Conventions consistent with Java.
		class ConsistentConvention : IObjectMappingConvention
		{
			public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> mapping)
			{
				// Java field naming convention.
				mapping.SetNamingConvention(new CamelCaseNamingConvention());
				// Do not serialize properties, only fields. Include both public and private fields.
				foreach (var field in mapping.ObjectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					if (field.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
						mapping.MapMember(field, field.FieldType);
			}
		}

		class ConsistentConventionProvider : IObjectMappingConventionProvider
		{
			public IObjectMappingConvention GetConvention(Type type) { return new ConsistentConvention(); }
		}

		static readonly CborOptions Options = new CborOptions();

		static SerializationUtils()
		{
			Options.Registry.ObjectMappingConventionRegistry.RegisterProvider(new ConsistentConventionProvider());
		}

		public static byte[] Serialize<T>(T data)
		{
			using (var buffer = new ByteBufferWriter())
			{
				Cbor.Serialize(data, buffer, Options);
				return buffer.WrittenSpan.ToArray();
			}
		}
	}
}
