using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;

namespace SourceAFIS.Tests
{
    static class Settings
    {
        public static string RootFolder = Path.GetFullPath(Path.Combine("..", "..", ".."));
        public static string DataPath = Path.Combine(RootFolder, "Data");
        public static string DatabasePath = Path.Combine(DataPath, "TestDatabase");
        public static string SomeDatabase = Path.Combine(DatabasePath, "FVC2000", "DB2_B");

        public static string SomeFingerprintPath = Path.Combine(SomeDatabase, "101_1.tif");
        public static string MatchingFingerprintPath = Path.Combine(SomeDatabase, "101_2.tif");
        public static string NonMatchingFingerprintPath = Path.Combine(SomeDatabase, "102_1.tif");

        public static BitmapImage SomeFingerprint = new BitmapImage(new Uri(Settings.SomeFingerprintPath, UriKind.RelativeOrAbsolute));
        public static BitmapImage MatchingFingerprint = new BitmapImage(new Uri(Settings.MatchingFingerprintPath, UriKind.RelativeOrAbsolute));
        public static BitmapImage NonMatchingFingerprint = new BitmapImage(new Uri(Settings.NonMatchingFingerprintPath, UriKind.RelativeOrAbsolute));

        public static string SavedImagePath = Path.GetFullPath(Path.Combine("saved", "saved{0}.png"));
        public static int SavedImageCounter;
        public static string LastSavedImage;
    }
}
