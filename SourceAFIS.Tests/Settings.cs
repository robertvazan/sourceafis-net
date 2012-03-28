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
        public static string JavaDataPath = Path.Combine(DataPath, "JavaTestData");
        public static string IsoTemplatePath = Path.Combine(DataPath, "IsoTemplates");

        public static string SomeFingerprintPath = Path.Combine(SomeDatabase, "101_1.tif");
        public static string MatchingFingerprintPath = Path.Combine(SomeDatabase, "101_2.tif");
        public static string NonMatchingFingerprintPath = Path.Combine(SomeDatabase, "102_1.tif");

        public static string JavaFingerprintDatabase = Path.Combine(DatabasePath, "FVC2000", "DB1_B");
        public static string JavaFingerprintProbePath = Path.Combine(JavaFingerprintDatabase, "102_2.tif");
        public static string JavaFingerprintCandidatePath = Path.Combine(JavaFingerprintDatabase, "102_3.tif");
        public static BitmapImage JavaFingerprintProbe = new BitmapImage(new Uri(JavaFingerprintProbePath, UriKind.RelativeOrAbsolute));
        public static BitmapImage JavaFingerprintCandidate = new BitmapImage(new Uri(JavaFingerprintCandidatePath, UriKind.RelativeOrAbsolute));

        public static BitmapImage SomeFingerprint = new BitmapImage(new Uri(SomeFingerprintPath, UriKind.RelativeOrAbsolute));
        public static BitmapImage MatchingFingerprint = new BitmapImage(new Uri(MatchingFingerprintPath, UriKind.RelativeOrAbsolute));
        public static BitmapImage NonMatchingFingerprint = new BitmapImage(new Uri(NonMatchingFingerprintPath, UriKind.RelativeOrAbsolute));

        public static string SavedImagePath = Path.GetFullPath(Path.Combine("saved", "saved{0}.png"));
        public static int SavedImageCounter;
        public static string LastSavedImage;
    }
}
