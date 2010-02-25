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
    public sealed class TestDatabase
    {
        public List<Database> Databases = new List<Database>();

        public sealed class Database
        {
            public string Path;
            public List<Finger> Fingers = new List<Finger>();
        }

        public sealed class Finger
        {
            public string Name;
            public List<View> Views = new List<View>();
        }

        public sealed class View
        {
            public string Path;
            public string FileName;
            public Template Template;
        }

        public IEnumerable<Finger> AllFingers
        {
            get
            {
                foreach (Database database in Databases)
                    foreach (Finger finger in database.Fingers)
                        yield return finger;
            }
        }

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
    }
}
