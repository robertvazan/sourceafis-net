using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Database;
using SourceAFIS.Templates;

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

            SaveDatabase(folder);
        }

        void SaveDatabase(string folder)
        {
            for (int databaseIndex = 0; databaseIndex < Templates.Databases.Count; ++databaseIndex)
                SaveTestDatabase(Templates.Databases[databaseIndex], Path.Combine(folder, "database" + (databaseIndex + 1).ToString()));
        }

        static readonly SerializedFormat SerializedFormat = new SerializedFormat();
        static readonly XmlFormat XmlFormat = new XmlFormat();

        void SaveTestDatabase(TestDatabase database, string folder)
        {
            Directory.CreateDirectory(folder);
            Parallel.ForEach(database.AllIndexes, (index) =>
            {
                TestDatabase.View view = database[index];
                XmlFormat.Export(SerializedFormat.Import(view.Template)).Save(Path.Combine(folder, view.FileName + ".xml"));
            });
        }
    }
}
