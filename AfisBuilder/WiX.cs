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
        static XmlElement Feature;

        static string SourceFolder;
        static List<string> Folders;
        static List<string> Files;

        public static void Load(string path)
        {
            Document = new XmlDocument();
            Document.Load(path);
            Root = Document.DocumentElement;
            Product = GetChildByName(Root, "Product");
            PFiles = GetPFilesDirectory();
            Feature = GetChildByName(Product, "Feature");
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

        public static void ScanFiles()
        {
            Files = new List<string>();
            foreach (string folder in Folders)
            {
                foreach (string path in Directory.GetFiles(SourceFolder + @"\" + folder))
                    Files.Add(folder + @"\" + Path.GetFileName(path));
            }
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

        static XmlElement GetChildByNameAttr(XmlElement parent, string name)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.GetAttribute("Name") == name)
                        return element;
                }
            }
            throw new ApplicationException("No element with such Id.");
        }

        static bool HasChildWithNameAttr(XmlElement parent, string name)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.GetAttribute("Name") == name)
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
                return GetChildByNameAttr(parent, Path.GetFileName(path));
            }
        }

        public static void AddMissingFolders()
        {
            foreach (string folder in Folders)
            {
                XmlElement parent = GetDirectoryElement(Path.GetDirectoryName(folder));
                string name = Path.GetFileName(folder);
                string id = GetUniqueId(folder);
                if (!HasChildWithNameAttr(parent, name))
                {
                    XmlElement element = Document.CreateElement("Directory", "http://schemas.microsoft.com/wix/2006/wi");
                    element.SetAttribute("Id", id);
                    element.SetAttribute("Name", name);
                    parent.AppendChild(element);
                }
            }
        }

        static string FilterFilename(string file)
        {
            const int MaxLength = 70;
            const int EndLength = MaxLength / 2 - 1;
            string filtered = file.Replace(@"\", "_").Replace("{", "_").Replace("}", "_").Replace("`", "_");
            if (filtered.Length <= MaxLength)
                return filtered;
            else
                return filtered.Substring(0, EndLength) + "__" + filtered.Substring(filtered.Length - EndLength, EndLength);
        }

        static bool ContainsIdAlready(XmlElement element, string id)
        {
            if (element.GetAttribute("Id") == id)
                return true;
            foreach (XmlNode node in element.ChildNodes)
                if (node is XmlElement && ContainsIdAlready((XmlElement)node, id))
                    return true;
            return false;
        }

        static bool ContainsIdAlready(string id)
        {
            return ContainsIdAlready(Root, id);
        }

        static string GetUniqueId(string bare)
        {
            string filtered = FilterFilename(bare);
            if (!ContainsIdAlready(filtered))
                return filtered;
            for (int i = 2; i < 100; ++i)
            {
                string numbered = filtered + i.ToString();
                if (!ContainsIdAlready(numbered))
                    return numbered;
            }
            throw new ApplicationException("Cannot generate unique ID.");
        }

        public static bool ContainsFile(XmlElement directory, string name)
        {
            foreach (XmlNode node in directory.ChildNodes)
                if (node is XmlElement)
                {
                    XmlElement component = (XmlElement)node;
                    if (component.Name == "Component")
                    {
                        XmlElement file = GetChildByName(component, "File");
                        if (file.GetAttribute("Name") == name)
                            return true;
                    }
                }
            return false;
        }

        public static void AddMissingFiles()
        {
            foreach (string file in Files)
            {
                XmlElement directory = GetDirectoryElement(Path.GetDirectoryName(file));
                string name = Path.GetFileName(file);
                string fileid = GetUniqueId(file);
                string componentid = GetUniqueId("component_" + file);
                
                if (!ContainsFile(directory, name))
                {
                    XmlElement component = Document.CreateElement("Component", "http://schemas.microsoft.com/wix/2006/wi");
                    component.SetAttribute("Id", componentid);
                    component.SetAttribute("Guid", Guid.NewGuid().ToString());
                    directory.AppendChild(component);
                    
                    XmlElement fileref = Document.CreateElement("File", "http://schemas.microsoft.com/wix/2006/wi");
                    fileref.SetAttribute("Id", fileid);
                    fileref.SetAttribute("Name", name);
                    fileref.SetAttribute("DiskId", "1");
                    fileref.SetAttribute("Source", file);
                    fileref.SetAttribute("KeyPath", "yes");
                    component.AppendChild(fileref);

                    XmlElement componentref = Document.CreateElement("ComponentRef", "http://schemas.microsoft.com/wix/2006/wi");
                    componentref.SetAttribute("Id", componentid);
                    Feature.AppendChild(componentref);
                }
            }
        }

        static void RemoveOldFiles(XmlElement directory, string path)
        {
            foreach (XmlNode node in directory.ChildNodes)
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.Name == "Component")
                    {
                        XmlElement file = GetChildByName(element, "File");
                        if (!Files.Contains(path + file.GetAttribute("Name")))
                        {
                            string id = element.GetAttribute("Id");
                            XmlElement componentref = GetChildById(Feature, id);
                            Feature.RemoveChild(componentref);
                            directory.RemoveChild(node);
                        }
                    }
                    else if (element.Name == "Directory")
                        RemoveOldFiles(element, path + element.GetAttribute("Name") + @"\");
                }
        }

        public static void RemoveOldFiles()
        {
            RemoveOldFiles(PFiles, "");
        }

        static void RemoveOldFolders(XmlElement directory, string path)
        {
            foreach (XmlNode node in directory.ChildNodes)
                if (node is XmlElement)
                {
                    XmlElement child = (XmlElement)node;
                    if (child.Name == "Directory")
                    {
                        if (!Folders.Contains(path + child.GetAttribute("Name")))
                            directory.RemoveChild(child);
                        else
                            RemoveOldFiles(child, path + child.GetAttribute("Name") + @"\");
                    }
                }
        }

        public static void RemoveOldFolders()
        {
            RemoveOldFolders(PFiles, "");
        }
    }
}
