using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace SourceAFIS.Tests
{
    static class Settings
    {
        public static string RootFolder = Path.Combine("..", "..", "..");
        public static string DataPath = Path.Combine(RootFolder, "Data");
        public static string DatabasePath = Path.Combine(DataPath, "TestDatabase");
        public static string SomeDatabase = Path.Combine(DatabasePath, "FVC2000", "DB2_B");

        public static string SomeFingerprintPath = Path.Combine(SomeDatabase, "101_1.tif");
        public static string MatchingFingerprintPath = Path.Combine(SomeDatabase, "101_2.tif");
        public static string NonMatchingFingerprintPath = Path.Combine(SomeDatabase, "102_1.tif");

        public static Bitmap SomeFingerprint = new Bitmap(Bitmap.FromFile(Settings.SomeFingerprintPath));
        public static Bitmap MatchingFingerprint = new Bitmap(Bitmap.FromFile(Settings.MatchingFingerprintPath));
        public static Bitmap NonMatchingFingerprint = new Bitmap(Bitmap.FromFile(Settings.NonMatchingFingerprintPath));
    }
}
