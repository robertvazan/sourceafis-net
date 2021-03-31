// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Security.Cryptography;

namespace SourceAFIS.Cmd
{
    class DataHash
    {
        readonly IncrementalHash Hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        public void Add(byte[] data) { Hasher.AppendData(data); }
        public byte[] Compute() { return Hasher.GetHashAndReset(); }

        public static byte[] Of(byte[] data)
        {
            var hash = new DataHash();
            hash.Add(data);
            return hash.Compute();
        }
        public static byte[] Of(string mime, byte[] data) { return Of(SerializationUtils.Normalize(mime, data)); }
        public static string Format(byte[] data) { return Convert.ToBase64String(data).TrimEnd(new[] { '=' }).Replace('+', '-').Replace('/', '_'); }
    }
}
