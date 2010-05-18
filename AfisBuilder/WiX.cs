using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AfisBuilder
{
    class WiX
    {
        static XmlDocument Document;

        public static void Load(string path)
        {
            Document = new XmlDocument();
            Document.Load(path);
        }

        public static void Save(string path)
        {
            Document.Save(path);
        }
    }
}
