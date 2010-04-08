using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using SourceAFIS.Meta;
using SourceAFIS.Extraction;
using SourceAFIS.Matching;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class TestConfiguration
    {
        public ParameterSet Parameters;
        public DatabaseStatistics DatabaseStatistics = new DatabaseStatistics();

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

            using (FileStream stream = File.Open(Path.Combine(folder, "ParameterChanges.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ParameterSet));
                serializer.Serialize(stream, GetChanges());
            }
        }

        ParameterSet GetChanges()
        {
            ParameterSet original = new ParameterSet();
            original.Add(new ObjectTree(new Extractor(), "Extractor"));
            original.Add(new ObjectTree(new Matcher(), "Matcher"));
            return Parameters.GetDifferences(original);
        }
    }
}
