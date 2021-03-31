// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace SourceAFIS.Cmd
{
    class TransparencyStats
    {
        public string Mime;
        public int Count;
        public long Size;
        public long SizeNormalized;
        public byte[] Hash;
        public static TransparencyStats Of(string mime, byte[] data)
        {
            var stats = new TransparencyStats();
            stats.Mime = mime;
            stats.Count = 1;
            stats.Size = data.Length;
            var normalized = SerializationUtils.Normalize(mime, data);
            stats.SizeNormalized = normalized.Length;
            stats.Hash = DataHash.Of(normalized);
            return stats;
        }
        public static TransparencyStats Sum(List<TransparencyStats> list)
        {
            var sum = new TransparencyStats();
            sum.Mime = list[0].Mime;
            var hash = new DataHash();
            foreach (var stats in list)
            {
                sum.Count += stats.Count;
                sum.Size += stats.Size;
                sum.SizeNormalized += stats.SizeNormalized;
                hash.Add(stats.Hash);
            }
            sum.Hash = hash.Compute();
            return sum;
        }
        public class Row
        {
            public string Key;
            public TransparencyStats Stats;
        }
        public class Table
        {
            public List<Row> Rows = new List<Row>();
            public static Table Of(string key, string mime, byte[] data)
            {
                var row = new Row();
                row.Key = key;
                row.Stats = TransparencyStats.Of(mime, data);
                var table = new Table();
                table.Rows.Add(row);
                return table;
            }
            public static Table Sum(List<Table> list)
            {
                var groups = new Dictionary<string, List<TransparencyStats>>();
                var sum = new Table();
                foreach (var table in list)
                {
                    foreach (var row in table.Rows)
                    {
                        List<TransparencyStats> group;
                        if (!groups.TryGetValue(row.Key, out group))
                        {
                            var srow = new Row();
                            srow.Key = row.Key;
                            sum.Rows.Add(srow);
                            groups[row.Key] = group = new List<TransparencyStats>();
                        }
                        group.Add(row.Stats);
                    }
                }
                foreach (var row in sum.Rows)
                    row.Stats = TransparencyStats.Sum(groups[row.Key]);
                return sum;
            }
        }
        class TableCollector : FingerprintTransparency
        {
            readonly List<Table> Records = new List<Table>();
            public override void Take(string key, string mime, byte[] data) { Records.Add(Table.Of(key, mime, data)); }
            public Table Sum() { return Table.Sum(Records); }
        }
        public static Table ExtractorTable(SampleFingerprint fp)
        {
            return PersistentCache.Get<Table>("extractor-transparency-stats", fp.Path, () =>
            {
                using (var collector = new TableCollector())
                {
                    new FingerprintTemplate(fp.Decode());
                    return collector.Sum();
                }
            });
        }
        public static TransparencyStats ExtractorRow(SampleFingerprint fp, String key) { return ExtractorTable(fp).Rows.Where(r => r.Key == key).First().Stats; }
        public static Table ExtractorTable() { return Table.Sum(SampleFingerprint.All.Select(fp => ExtractorTable(fp)).ToList()); }
        public static void Report(Table table)
        {
            foreach (var row in table.Rows)
            {
                var stats = row.Stats;
                Log.Information("Transparency/{Key}: {Mime}, {Count}x, {Size} B (normalized {SizeNormalized} B), hash {Hash}",
                    row.Key, stats.Mime, stats.Count, stats.Size / stats.Count, stats.SizeNormalized / stats.Count, DataHash.Format(stats.Hash));
            }
        }
    }
}
