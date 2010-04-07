using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning
{
    [Serializable]
    public sealed class TestDatabase : ICloneable
    {
        public List<Database> Databases = new List<Database>();

        [Serializable]
        public sealed class Database
        {
            public string Path;
            public List<Finger> Fingers = new List<Finger>();
        }

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

        [XmlIgnore]
        public IEnumerable<Finger> AllFingers
        {
            get
            {
                foreach (Database database in Databases)
                    foreach (Finger finger in database.Fingers)
                        yield return finger;
            }
        }

        [XmlIgnore]
        public IEnumerable<View> AllViews
        {
            get
            {
                foreach (Database database in Databases)
                    foreach (Finger finger in database.Fingers)
                        foreach (View view in finger.Views)
                            yield return view;
            }
        }

        public void Scan(string path)
        {
            List<string> files = new List<string>();
            foreach (string extension in new string[] { "bmp", "png", "jpg", "jpeg", "tif" })
                files.AddRange(Directory.GetFiles(path, "*_*." + extension));

            if (files.Count > 0)
            {
                files.Sort();
                Database database = new Database();
                Databases.Add(database);
                database.Path = path;

                foreach (string filePath in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(filePath);
                    int underscore = filename.LastIndexOf('_');
                    string fingerName = filename.Substring(0, underscore);
                    
                    Finger finger;
                    if (database.Fingers.Count == 0 || database.Fingers[database.Fingers.Count - 1].Name != fingerName)
                    {
                        finger = new Finger();
                        database.Fingers.Add(finger);
                        finger.Name = fingerName;
                    }
                    else
                        finger = database.Fingers[database.Fingers.Count - 1];

                    View view = new View();
                    finger.Views.Add(view);
                    view.Path = filePath;
                    view.FileName = filename;
                }
            }

            List<string> subfolders = new List<string>(Directory.GetDirectories(path));
            if (subfolders.Count > 0)
            {
                subfolders.Sort();
                foreach (string subfolder in subfolders)
                    Scan(subfolder);
            }
        }

        void ClipList<T>(List<T> list, int max)
        {
            if (list.Count > max)
                list.RemoveRange(max, list.Count - max);
        }

        public void ClipDatabaseCount(int max)
        {
            ClipList(Databases, max);
        }

        public void ClipFingersPerDatabase(int max)
        {
            foreach (Database database in Databases)
                ClipList(database.Fingers, max);
        }

        public void ClipViewsPerFinger(int max)
        {
            foreach (Finger finger in AllFingers)
                ClipList(finger.Views, max);
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
                TestDatabase loaded = (TestDatabase)formatter.Deserialize(stream);
                Databases = loaded.Databases;
                loaded.Databases = null;
            }
        }

        public TestDatabase Clone()
        {
            TestDatabase clone = new TestDatabase();
            foreach (Database database in Databases)
            {
                Database cloneDatabase = new Database();
                cloneDatabase.Path = database.Path;
                foreach (Finger finger in database.Fingers)
                {
                    Finger cloneFinger = new Finger();
                    cloneFinger.Name = finger.Name;
                    foreach (View view in finger.Views)
                    {
                        View cloneView = new View();
                        cloneView.FileName = view.FileName;
                        cloneView.Path = view.Path;
                        if (view.Template != null)
                            cloneView.Template = view.Template.Clone();
                        cloneFinger.Views.Add(cloneView);
                    }
                    cloneDatabase.Fingers.Add(cloneFinger);
                }
                clone.Databases.Add(cloneDatabase);
            }
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }

        public int GetFingerCount()
        {
            int count = 0;
            foreach (Database database in Databases)
                count += database.Fingers.Count;
            return count;
        }

        public int GetFingerprintCount()
        {
            int count = 0;
            foreach (Finger finger in AllFingers)
                count += finger.Views.Count;
            return count;
        }

        public int GetMatchingPairCount()
        {
            int count = 0;
            foreach (Finger finger in AllFingers)
                count += finger.Views.Count * (finger.Views.Count - 1);
            return count;
        }

        public int GetNonMatchingPairCount()
        {
            int count = 0;
            foreach (Database database in Databases)
                foreach (Finger finger in database.Fingers)
                    for (int viewIndex = 0; viewIndex < finger.Views.Count; ++viewIndex)
                        foreach (Finger pairFinger in database.Fingers)
                            if (pairFinger != finger && viewIndex < pairFinger.Views.Count)
                                ++count;
            return count;
        }
    }
}
