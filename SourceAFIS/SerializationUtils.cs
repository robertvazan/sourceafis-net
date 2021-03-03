// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using Dahomey.Cbor;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using Dahomey.Cbor.Util;

namespace SourceAFIS
{
	static class SerializationUtils
	{
		// Field naming convention consistent with Java.
		class ConsistentConvention : IObjectMappingConvention
		{
			static readonly DefaultObjectMappingConvention Defaults = new DefaultObjectMappingConvention();

			public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> mapping)
			{
				Defaults.Apply(registry, mapping);
				mapping.SetNamingConvention(new CamelCaseNamingConvention());
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
