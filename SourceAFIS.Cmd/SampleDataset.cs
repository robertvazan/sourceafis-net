// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SourceAFIS.Cmd
{
    class SampleDataset
    {
        public readonly String Name;
        public readonly SampleDownload.Format Format;
        public readonly double Dpi;
        public readonly SampleLayout Layout;
        static double LookupDpi(string dataset)
        {
            switch (dataset)
            {
                case "fvc2002-2b":
                    return 569;
                case "fvc2004-3b":
                    return 512;
                default:
                    return 500;
            }
        }
        SampleDataset(string name, SampleDownload.Format format)
        {
            Name = name;
            Format = format;
            Dpi = LookupDpi(name);
            Layout = new SampleLayout(SampleDownload.Unpack(name, format));
        }
        static readonly ConcurrentDictionary<Tuple<string, SampleDownload.Format>, SampleDataset> Cache = new ConcurrentDictionary<Tuple<string, SampleDownload.Format>, SampleDataset>();
        public static SampleDataset Get(string name, SampleDownload.Format format) { return Cache.GetOrAdd(Tuple.Create(name, format), t => new SampleDataset(t.Item1, t.Item2)); }
        public static List<SampleDataset> AllInFormat(SampleDownload.Format format) { return SampleDownload.Available.Select(n => Get(n, format)).ToList(); }
        public static List<SampleDataset> All { get { return AllInFormat(SampleDownload.DefaultFormat); } }
        public List<SampleFingerprint> Fingerprints { get { return Enumerable.Range(0, Layout.Fingerprints).Select(n => new SampleFingerprint(this, n)).ToList(); } }
        public string Path { get { return Name; } }
    }
}
