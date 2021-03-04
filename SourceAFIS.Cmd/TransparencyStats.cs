// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Serilog;

namespace SourceAFIS.Cmd
{
	class TransparencyStats
	{
		public int Count;
		public long Size;
		public byte[] Hash;

		public class Row : TransparencyStats {
			public string Key;
		}
		public class Table {
			public List<Row> Rows;
		}
		class Accumulator {
			public int Count;
			public long Size;
			public readonly IncrementalHash Hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

			public void Add(byte[] data)
			{
				++Count;
				Size += data.Length;
				Hasher.AppendData(data);
			}
			public void Add(TransparencyStats stats)
			{
				Count += stats.Count;
				Size += stats.Size;
				Hasher.AppendData(stats.Hash);
			}
		}
		class AccumulatorTable
		{
			readonly Dictionary<string, Accumulator> Accumulators = new Dictionary<string, Accumulator>();
			readonly List<string> Order = new List<string>();

			Accumulator Lookup(string key) {
				Accumulator accumulator;
				if (!Accumulators.TryGetValue(key, out accumulator)) {
					Order.Add(key);
					Accumulators[key] = accumulator = new Accumulator();
				}
				return accumulator;
			}
			public void Add(string key, byte[] data) { Lookup(key).Add(data); }
			public void Add(Row row) { Lookup(row.Key).Add(row); }
			public void Add(Table table)
			{
				foreach (var row in table.Rows)
					Add(row);
			}
			public Table Summarize()
			{
				var table = new Table();
				table.Rows = Order
					.Select(k => {
						var accumulator = Accumulators[k];
						var row = new Row();
						row.Key = k;
						row.Count = accumulator.Count;
						row.Size = accumulator.Size;
						row.Hash = accumulator.Hasher.GetHashAndReset();
						return row;
					})
					.ToList();
				return table;
			}
		}
		class TableCollector : FingerprintTransparency
		{
			public readonly AccumulatorTable Accumulator = new AccumulatorTable();

			public override void Take(string key, string mime, byte[] data) { Accumulator.Add(key, data); }
		}

		static Table SumTables(List<Table> tables)
		{
			var accumulator = new AccumulatorTable();
			foreach (var table in tables)
				accumulator.Add(table);
			return accumulator.Summarize();
		}
		public static Table ExtractorTable(SampleFingerprint fp)
		{
			return PersistentCache.Get<Table>("extractor-transparency-stats", fp, () =>
			{
				var image = fp.Load();
				using (var collector = new TableCollector())
				{
					new FingerprintTemplate(fp.Decode());
					return collector.Accumulator.Summarize();
				}
			});
		}
		public static Table ExtractorTable(SampleDataset dataset) { return SumTables(dataset.Fingerprints.Select(fp => ExtractorTable(fp)).ToList()); }
		public static Table ExtractorTable() { return SumTables(SampleDataset.All.Select(ds => ExtractorTable(ds)).ToList()); }
		static string UrlBase64(byte[] data) { return Convert.ToBase64String(data).TrimEnd(new[] { '=' }).Replace('+', '-').Replace('/', '_'); }
		public static void Report(Table table)
		{
			foreach (var row in table.Rows)
				Log.Information("{Key}: {Count}x, {Size} B, hash {Hash}", row.Key, row.Count, row.Size, UrlBase64(row.Hash));
		}
	}
}
