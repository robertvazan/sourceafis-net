using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SourceAFIS.Tuning.Database
{
    public struct DatabaseIndex
    {
        [XmlAttribute]
        public int Finger;
        [XmlAttribute]
        public int View;

        public DatabaseIndex(int finger, int view)
        {
            Finger = finger;
            View = view;
        }
    }
}
