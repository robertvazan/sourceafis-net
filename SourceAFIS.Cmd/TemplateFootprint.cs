// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace SourceAFIS.Cmd
{
    class TemplateFootprint
    {
        public int Count;
        public int Serialized;
        public byte[] Hash;
        public static TemplateFootprint Of(SampleFingerprint fp)
        {
            return PersistentCache.Get("footprints", fp.Path, () =>
            {
                var footprint = new TemplateFootprint();
                var serialized = NativeTemplate.Serialized(fp);
                footprint.Count = 1;
                footprint.Serialized = serialized.Length;
                footprint.Hash = DataHash.Of(SerializationUtils.Normalize(serialized));
                return footprint;
            });
        }
        public static TemplateFootprint Sum(List<TemplateFootprint> list)
        {
            var sum = new TemplateFootprint();
            var hash = new DataHash();
            foreach (var footprint in list)
            {
                sum.Count += footprint.Count;
                sum.Serialized += footprint.Serialized;
                hash.Add(footprint.Hash);
            }
            sum.Hash = hash.Compute();
            return sum;
        }
        public static TemplateFootprint Sum() { return Sum(SampleFingerprint.All.Select(fp => Of(fp)).ToList()); }
        public static void Report()
        {
            var sum = Sum();
            Log.Information("Template footprint: {Serialized} B serialized", sum.Serialized / sum.Count);
            Log.Information("Template hash: {Hash}", DataHash.Format(sum.Hash));
        }
    }
}
