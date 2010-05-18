using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace AfisBuilder
{
    class WiX
    {
        static XmlDocument Document;
        static XmlElement Root;
        static XmlElement Product;
        static XmlElement PFiles;

        static string SourceFolder;
        static List<string> Folders;

        public static void Load(string path)
        {
            Document = new XmlDocument();
            Document.Load(path);
            Root = Document.DocumentElement;
            Product = GetChildByName(Root, "Product");
            PFiles = GetPFilesDirectory();
        }

        public static void Save(string path)
        {
            Document.Save(path);
        }

        static void ScanFolders(string where, string prefix)
        {
            foreach (string subfolder in Directory.GetDirectories(where))
            {
                string name = Path.GetFileName(subfolder);
                Folders.Add(prefix + name);
                ScanFolders(where + @"\" + name, prefix + name + @"\");
            }
        }

        public static void ScanFolders(string path)
        {
            SourceFolder = path;
            Folders = new List<string>();
            ScanFolders(SourceFolder, "");
        }

        static XmlElement GetChildByName(XmlElement parent, string name)
        {
            XmlNodeList list = parent.GetElementsByTagName(name);
            if (list.Count == 0)
                throw new ApplicationException("No element with such name.");
            if (list.Count > 1)
                throw new ApplicationException("Multiple elements with such name.");
            return (XmlElement)list.Item(0);
        }

        static XmlElement GetChildById(XmlElement parent, string id)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.GetAttribute("Id") == id)
                        return element;
                }
            }
            throw new ApplicationException("No element with such Id.");
        }

        static bool HasChildWithId(XmlElement parent, string id)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.GetAttribute("Id") == id)
                        return true;
                }
            }
            return false;
        }

        static XmlElement GetPFilesDirectory()
        {
            XmlElement targetdir = GetChildById(Product, "TARGETDIR");
            XmlElement pffolder = GetChildById(targetdir, "ProgramFilesFolder");
            return GetChildById(pffolder, "INSTALLDIR");
        }

        static XmlElement GetDirectoryElement(string path)
        {
            if (path == "")
                return PFiles;
            else
            {
                XmlElement parent = GetDirectoryElement(Path.GetDirectoryName(path));
                return GetChildById(parent, Path.GetFileName(path));
            }
        }

        public static void AddMissingFolders()
        {
            foreach (string folder in Folders)
            {
                XmlElement parent = GetDirectoryElement(Path.GetDirectoryName(folder));
                string name = Path.GetFileName(folder);
                if (!HasChildWithId(parent, name))
                {
                    XmlElement element = Document.CreateElement("Directory", "http://schemas.microsoft.com/wix/2006/wi");
                    element.SetAttribute("Id", name);
                    element.SetAttribute("Name", name);
                    parent.AppendChild(element);
                }
            }
        }
    }
}
