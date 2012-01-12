using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Tuning.Database
{
    [Serializable]
    public sealed class TestDatabase : DatabaseLayout, ICloneable
    {
        public string DatabasePath;
        public List<Finger> Fingers;
        public int Dpi;
        public int MaxMatchingPerProbe = Int32.MaxValue;
        public int MaxNonMatchingPerProbe = Int32.MaxValue;

        public override int FingerCount { get { return Fingers.Count; } }
        public override int ViewCount { get { return Fingers[0].Views.Count; } }
        public override int MatchingPerProbe { get { return Math.Min(MaxMatchingPerProbe, ViewCount - 1); } }
        public override int NonMatchingPerProbe { get { return Math.Min(MaxNonMatchingPerProbe, (FingerCount - 1) * ViewCount); } }

		[XmlIgnore]
        public View this[DatabaseIndex index]
        {
            get { return Fingers[index.Finger].Views[index.View]; }
        }

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
                       let views = (from file in finger
                                    select new View(file.FilePath))
                       select new Finger(finger.Key, views)).ToList();

            int minViews = (from finger in Fingers
                            select finger.Views.Count).Min();

            ClipViews(minViews);
        }

        TestDatabase() { }

        public object Clone()
        {
            TestDatabase clone = new TestDatabase
            {
                DatabasePath = this.DatabasePath,
                Fingers = this.Fingers.CloneItems(),
                Dpi = this.Dpi,
                MaxMatchingPerProbe = this.MaxMatchingPerProbe,
                MaxNonMatchingPerProbe = this.MaxNonMatchingPerProbe
            };
            return clone;
        }

        public void ClipFingers(int limit)
        {
            Fingers.RemoveRange(limit);
        }

        public void ClipViews(int limit)
        {
            foreach (Finger finger in Fingers)
                finger.Views.RemoveRange(limit);
        }

        [Serializable]
        public sealed class Finger : ICloneable
        {
            public string Name;
            public List<View> Views;

            public Finger(string name, IEnumerable<View> views)
            {
                Name = name;
                Views = views.ToList();
            }

            private Finger() { }

            public object Clone() { return new Finger { Name = this.Name, Views = this.Views.CloneItems() }; }
        }

        [Serializable]
        public sealed class View : ICloneable
        {
            public string FilePath;
            public string FileName;
            [XmlIgnore]
            public Template Template;

            public View() { }

            public View(string path)
            {
                FilePath = path;
                FileName = Path.GetFileNameWithoutExtension(path);
            }

            public object Clone() { return this.ShallowClone(); }
        }
    }
}
