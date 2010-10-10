using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning.Database
{
    [Serializable]
    public sealed class DatabaseCollection : ICloneable
    {
        public List<TestDatabase> Databases = new List<TestDatabase>();

        [XmlIgnore]
        public int FingerCount { get { return Databases.Sum(db => db.FingerCount); } }

        [XmlIgnore]
        public int FpCount { get { return Databases.Sum(db => db.FpCount); } }

        public void Scan(string path)
        {
            List<string> files = (from extension in new string[] { "bmp", "png", "jpg", "jpeg", "tif" }
                                  from filepath in Directory.GetFiles(path, "*_*." + extension)
                                  orderby Path.GetFileNameWithoutExtension(filepath).ToLower()
                                  select filepath).ToList();

            if (files.Count > 0)
                Databases.Add(new TestDatabase(files));

            var subfolders = from subfolder in Directory.GetDirectories(path)
                             orderby Path.GetFileNameWithoutExtension(subfolder).ToLower()
                             select subfolder;
            foreach (string subfolder in subfolders)
                Scan(subfolder);
        }

        public void ClipDatabaseCount(int max)
        {
            Databases.RemoveRange(max);
        }

        public void ClipFingersPerDatabase(int max)
        {
            foreach (TestDatabase database in Databases)
                database.ClipFingers(max);
        }

        public void ClipViewsPerFinger(int max)
        {
            foreach (TestDatabase database in Databases)
                database.ClipViews(max);
        }

        public void Save(string path)
        {
            File.Delete(path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public void Load(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DatabaseCollection loaded = formatter.Deserialize(stream) as DatabaseCollection;
                Databases = loaded.Databases;
                loaded.Databases = null;
            }
        }

        public DatabaseCollection Clone()
        {
            return new DatabaseCollection { Databases = this.Databases.CloneItems() };
        }

        object ICloneable.Clone() { return Clone(); }

        public int GetMatchingPairCount()
        {
            return Databases.Sum(db => db.Fingers.Count * db.ViewCount * (db.ViewCount - 1));
        }

        public int GetNonMatchingPairCount()
        {
            return Databases.Sum(db => db.ViewCount * db.Fingers.Count * (db.Fingers.Count - 1));
        }
    }
}
