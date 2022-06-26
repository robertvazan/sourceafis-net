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
        public static byte[] Png() => Load("probe.png");
        public static byte[] Jpeg() => Load("probe.jpeg");
        public static byte[] Bmp() => Load("probe.bmp");
        public static byte[] Probe() => Load("probe.png");
        public static byte[] Matching() => Load("matching.png");
        public static byte[] Nonmatching() => Load("nonmatching.png");
        public static byte[] ProbeGray() => Load("gray-probe.dat");
        public static byte[] MatchingGray() => Load("gray-matching.dat");
        public static byte[] NonmatchingGray() => Load("gray-nonmatching.dat");
    }
}
