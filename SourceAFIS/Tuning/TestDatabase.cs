using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Tuning
{
    [Serializable]
    public sealed class TestDatabase : ICloneable
    {
        public string DatabasePath;
        public List<Finger> Fingers;
        public List<View> Views;

        public TestDatabase(List<string> files)
        {
            DatabasePath = Path.GetDirectoryName(files[0]);

            var details = from filepath in files
                          let filename = Path.GetFileNameWithoutExtension(filepath)
                          select new
                          {
                              FilePath = filepath,
                              FingerName = filename.Substring(0, filename.LastIndexOf('_'))
                          };

            Fingers = (from file in details
                       group file by file.FingerName into finger
                       let fingerprints = (from file in finger
                                           select new Fingerprint(file.FilePath))
                       select new Finger(finger.Key, fingerprints)).ToList();

            int minViews = (from finger in Fingers
                            select finger.Fingerprints.Count).Min();

            InitViews(minViews);
            ClipViews(minViews);
        }

        void InitViews(int count)
        {
            Views = (from index in Enumerable.Range(0, count)
                     let fingerprints = (from finger in Fingers
                                         select finger.Fingerprints[index])
                     select new View(index, fingerprints)).ToList();
        }

        TestDatabase() { }

        public object Clone()
        {
            TestDatabase clone = new TestDatabase
            {
                DatabasePath = this.DatabasePath,
                Fingers = this.Fingers.CloneItems()
            };
            clone.InitViews(Views.Count);
            return clone;
        }

        public void ClipFingers(int limit)
        {
            Fingers.RemoveRange(limit);
            foreach (View view in Views)
                view.Fingerprints.RemoveRange(limit);
        }

        public void ClipViews(int limit)
        {
            Views.RemoveRange(limit);
            foreach (Finger finger in Fingers)
                finger.Fingerprints.RemoveRange(limit);
        }

        [Serializable]
        public sealed class Finger : ICloneable
        {
            public string Name;
            public List<Fingerprint> Fingerprints;

            public Finger(string name, IEnumerable<Fingerprint> fingerprints)
            {
                Name = name;
                Fingerprints = fingerprints.ToList();
            }

            private Finger() { }

            public object Clone() { return new Finger { Name = this.Name, Fingerprints = this.Fingerprints.CloneItems() }; }
        }

        [Serializable]
        public sealed class View
        {
            public readonly int Index;
            public List<Fingerprint> Fingerprints;

            public View(int index, IEnumerable<Fingerprint> fingerprints)
            {
                Index = index;
                Fingerprints = fingerprints.ToList();
            }
        }

        [Serializable]
        public sealed class Fingerprint : ICloneable
        {
            public string FilePath;
            public string FileName;
            [XmlIgnore]
            public Template Template;

            public Fingerprint(string path)
            {
                FilePath = path;
                FileName = Path.GetFileNameWithoutExtension(path);
            }

            public object Clone() { return this.ShallowClone(); }
        }
    }
}
