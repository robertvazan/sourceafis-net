using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using System.IO;
using System.Xml.Serialization;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class TestConfiguration
    {
        public ParameterSet Parameters;

        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.Open(Path.Combine(folder, "Configuration.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestConfiguration));
                serializer.Serialize(stream, this);
            }

            using (FileStream stream = File.Open(Path.Combine(folder, "Parameters.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ParameterSet));
                serializer.Serialize(stream, Parameters);
            }
        }
    }
}
