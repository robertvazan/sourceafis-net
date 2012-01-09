using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.IO;

namespace AfisBuilder
{
    class Analyzer
    {
        public static string DatabasePath = "TestDatabase";
        public static float Accuracy;
        public static float Speed;
        public static float ExtractionTime;
        public static float TemplateSize;

        public static void PrepareXmlConfiguration(string sourcePath, string targetPath)
        {
            XDocument document = XDocument.Load(sourcePath);
            document.Root.Element("test-database").Element("scan").SetValue(DatabasePath);
			if (Command.Mono)
	            document.Root.Element("extractor-benchmark").Element("max-seconds").SetValue(1800);
            document.Save(targetPath);
        }

        public static void ReadAccuracy()
        {
            XElement root = XDocument.Load(Path.Combine("Matcher", "Accuracy", "ZeroFAR", "Accuracy.xml")).Root;
            Accuracy = (float)root.Element("AverageError");
        }

        public static void ReadSpeed()
        {
            XElement root = XDocument.Load(Path.Combine("Matcher", "MatcherTime.xml")).Root;
            Speed = 1 / (float)root.Element("NonMatching");
        }

        public static void ReadExtractorStats()
        {
            XElement root = XDocument.Load(Path.Combine("Extractor", "ExtractorReport.xml")).Root;
            ExtractionTime = (float)root.Element("Time");
            TemplateSize = (float)root.Element("TemplateSize");
        }

        public static void ReportStatistics()
        {
            Console.WriteLine("DatabaseAnalyzer results:");
            Console.WriteLine("    FRR: {0:F2}%", Accuracy * 100);
            Console.WriteLine("    Speed: {0:F0} fp/s", Speed);
            Console.WriteLine("    Extraction time: {0:F0}ms", ExtractionTime * 1000);
            Console.WriteLine("    Template size: {0:F2} KB", TemplateSize / 1024);
        }
    }
}
