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
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Tuning.Database;
using SourceAFIS.Matching;
using SourceAFIS.Matching.Minutia;
using SourceAFIS.Extraction;

namespace SourceAFIS.Tests.Executable
{
    [TestFixture]
    class JavaData
    {
        DatabaseCollection LoadDatabase()
        {
            DatabaseCollection db = new DatabaseCollection();
            db.Scan(Settings.DatabasePath);
            db.ClipFingersPerDatabase(8);
            db.ClipViewsPerFinger(8);
            return db;
        }

        Person ToPerson(Template template)
        {
            return new Person(new Fingerprint() { Template = new CompactFormat().SerializeBuilder(new SerializedFormat().Import(template)) });
        }

        void SaveXml(XElement root, string name)
        {
            Directory.CreateDirectory(Settings.JavaDataPath);
            new XDocument(root).Save(Path.Combine(Settings.JavaDataPath, name));
        }

        [Test]
        [Category("Special")]
        public void Templates()
        {
            XElement root = new XElement("template-list");
            AfisEngine afis = new AfisEngine();
            DatabaseCollection db = LoadDatabase();
            foreach (var database in db.Databases)
                foreach (var index in database.AllIndexes)
                {
                    Fingerprint fp = new Fingerprint();
                    fp.AsBitmapSource = new BitmapImage(new Uri(database[index].FilePath, UriKind.RelativeOrAbsolute));
                    afis.Extract(new Person(fp));
                    root.Add(new XElement("template",
                        new XAttribute("image-path", database[index].FilePath),
                        new XAttribute("compact", Convert.ToBase64String(fp.Template))));
                }
            SaveXml(root, "templates.xml");
        }

        [Test]
        [Category("JavaData")]
        public void Scores()
        {
            var templates = XDocument.Load(Path.Combine(Settings.JavaDataPath, "templates.xml")).Root.Elements();
            XElement root = new XElement("score-list");
            AfisEngine afis = new AfisEngine();
            DatabaseCollection db = LoadDatabase();
            foreach (var database in db.Databases)
            {
                foreach (var index in database.AllIndexes)
                {
                    var template = (string)templates.First(t => (string)t.Attribute("image-path") == database[index].FilePath).Attribute("compact");
                    database[index].Template = new SerializedFormat().Export(new CompactFormat().Import(Convert.FromBase64String(template)));
                }
                foreach (var pair in database.AllPairs)
                {
                    float score = afis.Verify(ToPerson(database[pair.Probe].Template), ToPerson(database[pair.Candidate].Template));
                    root.Add(new XElement("pair",
                        new XAttribute("probe", database[pair.Probe].FilePath),
                        new XAttribute("candidate", database[pair.Candidate].FilePath),
                        new XAttribute("score", score)));
                }
            }
            SaveXml(root, "score.xml");
        }

        [Test]
        [Category("JavaData")]
        public void MatchLog()
        {
            Extractor extractor = new Extractor();
            ParallelMatcher matcher = new ParallelMatcher();
            DetailLogger logger = new DetailLogger();
            logger.Attach(new ObjectTree(matcher));

            Template probe = new Template(extractor.Extract(WpfIO.GetPixels(Settings.JavaFingerprintProbe), 500));
            Template candidate = new Template(extractor.Extract(WpfIO.GetPixels(Settings.JavaFingerprintCandidate), 500));
            ParallelMatcher.PreparedProbe prepared = matcher.Prepare(probe);
            matcher.Match(prepared, new[] { candidate });
            DetailLogger.LogData log = logger.PopLog();

            XElement root = new XElement("matcher");
            root.SetAttributeValue("probe", Settings.JavaFingerprintProbePath);
            root.SetAttributeValue("candidate", Settings.JavaFingerprintCandidatePath);
            root.SetAttributeValue("best-root", (int)log.Retrieve("MinutiaMatcher.BestRootIndex"));
            EdgeTable edgeTable = (EdgeTable)log.Retrieve("MinutiaMatcher.EdgeTablePrototype");
            for (int i = 0; i < probe.Minutiae.Length; ++i)
            {
                XElement edgeList = new XElement("edge-list",
                    new XAttribute("offset", i));
                root.Add(edgeList);
                var edges = edgeTable.Table[i];
                for (int j = 0; j < edges.Length; ++j)
                    edgeList.Add(new XElement("edge",
                        new XAttribute("neighbor", edges[j].Neighbor),
                        new XAttribute("length", edges[j].Edge.Length)));
            }

            for (int i = 0; i < log.Count("MinutiaMatcher.RootSelector"); ++i)
            {
                MinutiaPair rootPair = (MinutiaPair)log.Retrieve("MinutiaMatcher.RootSelector", i);
                var rootPairInXml = new XElement("root",
                    new XAttribute("offset", i),
                    new XAttribute("probe", rootPair.Probe),
                    new XAttribute("candidate", rootPair.Candidate),
                    new XAttribute("score", (float)log.Retrieve("MinutiaMatcher.MatchScoring", i)));
                root.Add(rootPairInXml);
                MinutiaPairing pairing = (MinutiaPairing)log.Retrieve("MinutiaMatcher.Pairing", i);
                for (int j = 0; j < pairing.Count; ++j)
                    rootPairInXml.Add(new XElement("pair",
                        new XAttribute("probe", pairing.GetPair(j).Probe),
                        new XAttribute("candidate", pairing.GetPair(j).Candidate),
                        new XAttribute("support", pairing.GetSupportByProbe(pairing.GetPair(j).Probe))));
            }
            SaveXml(root, "matcher.xml");
        }

        [Test]
        [Category("JavaData")]
        public void Parameters()
        {
            ParallelMatcher matcher = new ParallelMatcher();
            ParameterSet parameters = new ParameterSet(new ObjectTree(matcher));
            XElement root = new XElement("parameters");
            foreach (var param in parameters.AllParameters)
                root.Add(new XElement("param",
                    new XAttribute("path", param.FieldPath),
                    new XAttribute("value", param.Value.Double.ToString(String.Format("F{0}", param.Value.Precision), CultureInfo.InvariantCulture))));
            SaveXml(root, "parameters.xml");
        }
    }
}
