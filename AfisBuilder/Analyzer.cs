using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

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
            XmlDocument document = new XmlDocument();
            document.Load(sourcePath);

            XmlElement root = document.DocumentElement;
            XmlElement db = (XmlElement)(root.GetElementsByTagName("test-database")[0]);
            XmlElement scan = (XmlElement)(root.GetElementsByTagName("scan")[0]);
            scan.InnerText = DatabasePath;

            document.Save(targetPath);
        }

        public static void ReadAccuracy()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Command.FixPath(@"Matcher\Accuracy\Standard\Accuracy.xml"));
            XmlElement root = document.DocumentElement;
            XmlElement average = (XmlElement)(root.GetElementsByTagName("AverageError")[0]);
            Accuracy = Convert.ToSingle(average.InnerText, CultureInfo.InvariantCulture);
        }

        public static void ReadSpeed()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Command.FixPath(@"Matcher\MatcherTime.xml"));
            XmlElement root = document.DocumentElement;
            XmlElement average = (XmlElement)(root.GetElementsByTagName("NonMatching")[0]);
            Speed = 1 / Convert.ToSingle(average.InnerText, CultureInfo.InvariantCulture);
        }

        public static void ReadExtractorStats()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Command.FixPath(@"Extractor\ExtractorReport.xml"));
            XmlElement root = document.DocumentElement;
            
            XmlElement time = (XmlElement)(root.GetElementsByTagName("Time")[0]);
            ExtractionTime = Convert.ToSingle(time.InnerText, CultureInfo.InvariantCulture);

            XmlElement size = (XmlElement)(root.GetElementsByTagName("TemplateSize")[0]);
            TemplateSize = Convert.ToSingle(size.InnerText, CultureInfo.InvariantCulture);
        }

        public static void ReportStatistics()
        {
            Console.WriteLine("DatabaseAnalyzer results:");
            Console.WriteLine("    EER: {0:F2}%", Accuracy * 100);
            Console.WriteLine("    Speed: {0:F0} fp/s", Speed);
            Console.WriteLine("    Extraction time: {0:F0}ms", ExtractionTime * 1000);
            Console.WriteLine("    Template size: {0:F1} KB", TemplateSize / 1024);
        }
    }
}
