using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning
{
    public sealed class TestDatabase : IEnumerable<TestDatabase.Database>
    {
        public List<Database> Databases = new List<Database>();
        public IEnumerator<Database> GetEnumerator() { return Databases.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return Databases.GetEnumerator(); }

        public sealed class Database : IEnumerable<Finger>
        {
            public string Path;
            public List<Finger> Fingers = new List<Finger>();
            public IEnumerator<Finger> GetEnumerator() { return Fingers.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return Fingers.GetEnumerator(); }
        }

        public sealed class Finger : IEnumerable<View>
        {
            public string Name;
            public List<View> Views = new List<View>();
            public IEnumerator<View> GetEnumerator() { return Views.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return Views.GetEnumerator(); }
        }

        public sealed class View
        {
            public string Path;
            public string FileName;
            public Template Template;
        }

        public IEnumerable<View> AllViews
        {
            get
            {
                foreach (Database database in this)
                    foreach (Finger finger in database)
                        foreach (View view in finger)
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

        public void ClipFingersPerDatabase(int max)
        {
            foreach (Database database in this)
                if (database.Fingers.Count > max)
                    database.Fingers.RemoveRange(max, database.Fingers.Count - max);
        }
    }
}
