// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Serilog;

namespace SourceAFIS.Cmd
{
    class PersistentCache
    {
        public static readonly string Home = Path.GetFullPath(".cache");
        public static readonly string Output = Path.Combine(Home, "net", FingerprintCompatibility.Version);

        static PersistentCache()
        {
            Log.Information("Cache directory: {Dir}", Home);
            Log.Information("Library version: {Version}", FingerprintCompatibility.Version);
        }

        interface ISerialization
        {
            string Rename(string path);
            byte[] Serialize(object data);
            T Deserialize<T>(byte[] serialized);
        }
        class TrivialSerialization : ISerialization
        {
            public string Rename(string path) { return path; }
            public byte[] Serialize(object data) { return (byte[])data; }
            public T Deserialize<T>(byte[] serialized) { return (T)(object)serialized; }
        }
        class CborSerialization : ISerialization
        {
            public string Rename(string path) { return path + ".cbor"; }
            public byte[] Serialize(object data) { return SerializationUtils.Serialize(data); }
            public T Deserialize<T>(byte[] serialized) { return SerializationUtils.Deserialize<T>(serialized); }
        }
        static ISerialization Serialization<T>()
        {
            if (typeof(T) == typeof(byte[]))
                return new TrivialSerialization();
            return new CborSerialization();
        }

        interface ICompression
        {
            string Rename(string path);
            byte[] Compress(byte[] data);
            byte[] Decompress(byte[] compressed);
        }
        class TrivialCompression : ICompression
        {
            public string Rename(string path) { return path; }
            public byte[] Compress(byte[] data) { return data; }
            public byte[] Decompress(byte[] compressed) { return compressed; }
        }
        class GzipCompression : ICompression
        {
            public string Rename(string path) { return path + ".gz"; }
            public byte[] Compress(byte[] data)
            {
                using (var buffer = new MemoryStream())
                {
                    using (var gzip = new GZipStream(buffer, CompressionMode.Compress))
                        gzip.Write(data);
                    return buffer.ToArray();
                }
            }
            public byte[] Decompress(byte[] compressed)
            {
                using (var buffer = new MemoryStream())
                {
                    using (var input = new MemoryStream(compressed))
                    using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                        gzip.CopyTo(buffer);
                    return buffer.ToArray();
                }
            }
        }
        static ICompression Compression(string path)
        {
            if (path.EndsWith(".cbor"))
                return new GzipCompression();
            return new TrivialCompression();
        }

        static readonly HashSet<string> Reported = new HashSet<string>();
        public static T Get<T>(string category, string identity, Func<T> supplier)
        {
            var serialization = Serialization<T>();
            var compression = Compression(serialization.Rename(identity));
            var path = compression.Rename(serialization.Rename(Path.Combine(Output, category, identity)));
            if (File.Exists(path))
                return serialization.Deserialize<T>(compression.Decompress(File.ReadAllBytes(path)));
            lock (Reported)
            {
                if (!Reported.Contains(category))
                {
                    Reported.Add(category);
                    Log.Information("Computing {Category}...", category);
                }
            }
            T computed = supplier();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, compression.Compress(serialization.Serialize(computed)));
            return computed;
        }
    }
}
