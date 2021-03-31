// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DirectoryApi = System.IO.Directory;

namespace SourceAFIS.Cmd
{
    class SampleLayout
    {
        public readonly string Directory;
        readonly int[] OffsetArray;
        readonly int[] FingerArray;
        readonly String[] NameArray;
        readonly String[] FilenameArray;
        readonly String[] PrefixArray;
        public int Fingers { get { return OffsetArray.Length - 1; } }
        public int Impressions(int finger) { return OffsetArray[finger + 1] - OffsetArray[finger]; }
        public int Fingerprints { get { return FingerArray.Length; } }
        public int Fingerprint(int finger, int impression)
        {
            if (impression < 0 || impression >= Impressions(finger))
                throw new ArgumentOutOfRangeException();
            return OffsetArray[finger] + impression;
        }
        public int Finger(int fp) { return FingerArray[fp]; }
        public int Impression(int fp) { return fp - OffsetArray[Finger(fp)]; }
        public string Name(int fp) { return NameArray[fp]; }
        public string Filename(int fp) { return FilenameArray[fp]; }
        public string Prefix(int finger) { return PrefixArray[finger]; }
        static readonly Regex Pattern = new Regex(@"^(.+)_[0-9]+\.(?:tif|tiff|png|bmp|jpg|jpeg|wsq|gray)$");
        public SampleLayout(string directory)
        {
            Directory = directory;
            var groups = new Dictionary<string, List<string>>();
            foreach (var path in DirectoryApi.GetFiles(directory))
            {
                var filename = Path.GetFileName(path);
                var match = Pattern.Match(filename);
                if (match.Success)
                {
                    var prefix = match.Groups[1].Value;
                    if (!groups.ContainsKey(prefix))
                        groups[prefix] = new List<string>();
                    groups[prefix].Add(filename);
                }
            }
            if (groups.Count == 0)
                throw new Exception("Empty dataset.");
            if (groups.Count == 1)
                throw new Exception("Found only one finger in the dataset.");
            if (!groups.Values.Any(l => l.Count > 1))
                throw new Exception("Found only one impression per finger in the dataset.");
            PrefixArray = new string[groups.Count];
            NameArray = new string[groups.Values.Sum(l => l.Count)];
            FilenameArray = new string[NameArray.Length];
            OffsetArray = new int[PrefixArray.Length + 1];
            FingerArray = new int[NameArray.Length];
            int finger = 0;
            int fp = 0;
            foreach (var prefix in groups.Keys.OrderBy(k => k))
            {
                PrefixArray[finger] = prefix;
                OffsetArray[finger + 1] = OffsetArray[finger] + groups[prefix].Count;
                foreach (var filename in groups[prefix].OrderBy(f => f))
                {
                    FilenameArray[fp] = filename;
                    NameArray[fp] = Path.GetFileNameWithoutExtension(filename);
                    FingerArray[fp] = finger;
                    ++fp;
                }
                ++finger;
            }
        }
    }
}
