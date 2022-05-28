// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    static class TestResources
    {
        static byte[] Load(String name)
        {
            using (var stream = typeof(TestResources).Assembly.GetManifestResourceStream($"SourceAFIS.Resources.{name}"))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }
        public static byte[] Probe() { return Load("probe.png"); }
        public static byte[] Matching() { return Load("matching.png"); }
        public static byte[] Nonmatching() { return Load("nonmatching.png"); }
        public static byte[] ProbeGray() { return Load("gray-probe.dat"); }
        public static byte[] MatchingGray() { return Load("gray-matching.dat"); }
        public static byte[] NonmatchingGray() { return Load("gray-nonmatching.dat"); }
    }
}
