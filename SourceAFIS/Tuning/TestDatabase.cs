using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Tuning
{
    [Serializable]
    public sealed class TestDatabase
    {
        public string Path;
        public List<Finger> Fingers = new List<Finger>();

        [Serializable]
        public sealed class Finger
        {
            public string Name;
            public List<View> Views = new List<View>();
        }

        [Serializable]
        public sealed class View
        {
            public string Path;
            public string FileName;
            [XmlIgnore]
            public Template Template;
        }
    }
}
