using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.Globalization;
using NUnit.Framework;
using SourceAFIS.Simple;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tests.Executable
{
    [TestFixture]
    [Category("Special")]
    class JavaData
    {
        void ClipDatabase(DatabaseCollection db)
        {
            db.ClipDatabaseCount(3);
            db.ClipFingersPerDatabase(3);
            db.ClipViewsPerFinger(3);
        }

        Person ToPerson(Template template)
        {
            return new Person(new Fingerprint() { Template = new CompactFormat().SerializeBuilder(new SerializedFormat().Import(template)) });
        }

        [Test]
        public void Templates()
        {
            XDocument document = new XDocument(new XElement("template-list"));
            AfisEngine afis = new AfisEngine();
            DatabaseCollection db = new DatabaseCollection();
            db.Scan(Settings.DatabasePath);
            ClipDatabase(db);
            foreach (var database in db.Databases)
                foreach (var index in database.AllIndexes)
                {
                    Fingerprint fp = new Fingerprint();
                    fp.AsBitmapSource = new BitmapImage(new Uri(database[index].FilePath, UriKind.RelativeOrAbsolute));
                    afis.Extract(new Person(fp));
                    document.Root.Add(new XElement("template",
                        new XAttribute("image-path", database[index].FilePath),
                        new XAttribute("compact", Convert.ToBase64String(fp.Template))));
                }
            Directory.CreateDirectory(Settings.JavaDataPath);
            document.Save(Path.Combine(Settings.JavaDataPath, "templates.xml"));
        }

        [Test]
        public void Scores()
        {
            var templates = XDocument.Load(Path.Combine(Settings.JavaDataPath, "templates.xml")).Root.Elements();
            XDocument document = new XDocument(new XElement("score-list"));
            AfisEngine afis = new AfisEngine();
            DatabaseCollection db = new DatabaseCollection();
            db.Scan(Settings.DatabasePath);
            ClipDatabase(db);
            foreach (var database in db.Databases)
            {
                foreach (var index in database.AllIndexes)
                {
                    var template = (string)templates.First(t => (string)t.Attribute("image-path") == database[index].FilePath).Attribute("compact");
                    database[index].Template = new SerializedFormat().Export(new CompactFormat().Import(Convert.FromBase64String(template)));
                }
                foreach (var pair in database.AllPairs)
                {
                    double score = afis.Verify(ToPerson(database[pair.Probe].Template), ToPerson(database[pair.Candidate].Template));
                    document.Root.Add(new XElement("pair",
                        new XAttribute("left", database[pair.Probe].FilePath),
                        new XAttribute("right", database[pair.Candidate].FilePath),
                        new XAttribute("score", score.ToString("0.####", CultureInfo.InvariantCulture))));
                }
            }
            document.Save(Path.Combine(Settings.JavaDataPath, "score.xml"));
        }
    }
}
