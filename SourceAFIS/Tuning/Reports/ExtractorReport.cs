using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class ExtractorReport
    {
        public float Time;
        public float MinutiaCount;
        public float TemplateSize;

        [XmlIgnore]
        public DatabaseCollection Templates;

        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.Open(Path.Combine(folder, "ExtractorReport.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ExtractorReport));
                serializer.Serialize(stream, this);
            }

            using (FileStream stream = File.Open(Path.Combine(folder, "Templates.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCollection));
                serializer.Serialize(stream, Templates);
            }

            Templates.Save(Path.Combine(folder, "Templates.dat"));
        }
    }
}
