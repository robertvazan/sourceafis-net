// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using Dahomey.Cbor;
using Dahomey.Cbor.ObjectModel;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using Dahomey.Cbor.Util;

namespace SourceAFIS.Cmd
{
    static class SerializationUtils
    {
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
        public static byte[] Serialize(object data)
        {
            using (var buffer = new ByteBufferWriter())
            {
                Cbor.Serialize(data, data.GetType(), buffer, Options);
                return buffer.WrittenSpan.ToArray();
            }
        }
        public static T Deserialize<T>(byte[] bytes) { return Cbor.Deserialize<T>(bytes, Options); }
        static byte[] ToBytes(int number)
        {
            var bytes = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        static byte[] ToBytes(long number)
        {
            var bytes = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        static byte[] ToBytes(double number)
        {
            var bytes = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        static void Normalize(Stream stream, CborValue node)
        {
            switch (node.Type)
            {
                case CborValueType.Object:
                    var map = (CborObject)node;
                    stream.WriteByte(0xBF);
                    foreach (var key in map.Keys.Select(k => k.Value<string>()).OrderBy(k => k))
                    {
                        stream.WriteByte(0x78);
                        var utf = Encoding.UTF8.GetBytes(key);
                        stream.WriteByte(checked((byte)utf.Length));
                        stream.Write(utf);
                        Normalize(stream, map[key]);
                    }
                    stream.WriteByte(0xFF);
                    break;
                case CborValueType.Array:
                    var array = (CborArray)node;
                    stream.WriteByte(0x9F);
                    foreach (var item in array)
                        Normalize(stream, item);
                    stream.WriteByte(0xFF);
                    break;
                case CborValueType.Positive:
                    stream.WriteByte(0x1B);
                    stream.Write(ToBytes(node.Value<long>()));
                    break;
                case CborValueType.Negative:
                    stream.WriteByte(0x3B);
                    stream.Write(ToBytes(-node.Value<long>()));
                    break;
                case CborValueType.Single:
                case CborValueType.Double:
                    stream.WriteByte(0xFB);
                    stream.Write(ToBytes(node.Value<double>()));
                    break;
                case CborValueType.ByteString:
                    var bytes = node.Value<ReadOnlyMemory<byte>>().ToArray();
                    stream.WriteByte(0x5A);
                    stream.Write(ToBytes(bytes.Length));
                    stream.Write(bytes);
                    break;
                case CborValueType.String:
                    {
                        var utf = Encoding.UTF8.GetBytes(node.Value<string>());
                        stream.WriteByte(0x7A);
                        stream.Write(ToBytes(utf.Length));
                        stream.Write(utf);
                    }
                    break;
                case CborValueType.Boolean:
                    if (node.Value<bool>())
                        stream.WriteByte(0xF5);
                    else
                        stream.WriteByte(0xF4);
                    break;
                case CborValueType.Null:
                    stream.WriteByte(0xF6);
                    break;
                case CborValueType.Decimal:
                case CborValueType.Undefined:
                default:
                    throw new ArgumentException();
            }
        }
        public static byte[] Normalize(byte[] denormalized)
        {
            var root = Cbor.Deserialize<CborValue>(denormalized);
            var buffer = new MemoryStream();
            Normalize(buffer, root);
            return buffer.ToArray();
        }
        public static byte[] Normalize(string mime, byte[] data)
        {
            if (mime == "application/cbor")
                return Normalize(data);
            return data;
        }
    }
}
