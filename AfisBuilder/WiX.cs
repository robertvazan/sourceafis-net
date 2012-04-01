using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace AfisBuilder
{
    static class WiX
    {
        static XNamespace Ns = "http://schemas.microsoft.com/wix/2006/wi";
        static XDocument Document;
        static XElement Root;
        static XElement Product;
        static XElement PFiles;
        static XElement Feature;

        static string SourceFolder;
        static List<string> Folders;
        static List<string> Files;

        public static void Load(string path)
        {
            Document = XDocument.Load(path);
            Root = Document.Root;
            Product = Root.WixElement("Product");
            PFiles = Product.ElementById("TARGETDIR").ElementById("ProgramFilesFolder").ElementById("INSTALLDIR");
            Feature = Product.WixElement("Feature");
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
                ScanFolders(Path.Combine(where, name), prefix + name + @"\");
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
                foreach (string path in Directory.GetFiles(Path.Combine(SourceFolder, folder)))
                    Files.Add(Path.Combine(folder, Path.GetFileName(path)));
            }
        }

        static XElement ElementById(this XElement parent, string id)
        {
            return (from child in parent.Elements()
                    where (string)child.Attribute("Id") == id
                    select child).FirstOrDefault();
        }

        static XElement ElementByNameAttribute(this XElement parent, string name)
        {
            return (from child in parent.Elements()
                    where (string)child.Attribute("Name") == name
                    select child).FirstOrDefault();
        }

        static XElement GetDirectoryElement(string path)
        {
            if (path == "")
                return PFiles;
            else
            {
                XElement parent = GetDirectoryElement(Path.GetDirectoryName(path));
                return parent.ElementByNameAttribute(Path.GetFileName(path));
            }
        }

        public static void AddMissingFolders()
        {
            foreach (string folder in Folders)
            {
                XElement parent = GetDirectoryElement(Path.GetDirectoryName(folder));
                string name = Path.GetFileName(folder);
                string id = GetUniqueId(folder);
                if (parent != null && parent.ElementByNameAttribute(name) == null)
                {
                    parent.Add(new XElement(Ns + "Directory",
                        new XAttribute("Id", id),
                        new XAttribute("Name", name)));
                }
            }
        }

        static string FilterFilename(string file)
        {
            const int MaxLength = 70;
            const int EndLength = MaxLength / 2 - 1;
            string filtered = file.Replace(@"\", "_").Replace("{", "_").Replace("}", "_").Replace("`", "_").Replace("-", "_");
            if (filtered.Length <= MaxLength)
                return filtered;
            else
                return filtered.Substring(0, EndLength) + "__" + filtered.Substring(filtered.Length - EndLength, EndLength);
        }

        static bool ContainsIdAlready(XElement element, string id)
        {
            if ((string)element.Attribute("Id") == id)
                return true;
            return element.Elements().Any(child => ContainsIdAlready(child, id));
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

        public static bool ContainsFile(XElement directory, string name)
        {
            return (from component in directory.WixElements("Component")
                    let file = component.WixElement("File")
                    where file != null && (string)file.Attribute("Name") == name
                    select component).Any();
        }

        public static void AddMissingFiles()
        {
            foreach (string file in Files)
            {
                XElement directory = GetDirectoryElement(Path.GetDirectoryName(file));
                string name = Path.GetFileName(file);
                string fileid = GetUniqueId(file);
                string componentid = GetUniqueId("component_" + file);
                
                if (!ContainsFile(directory, name))
                {
                    directory.Add(new XElement("{http://schemas.microsoft.com/wix/2006/wi}Component",
                        new XAttribute("Id", componentid),
                        new XAttribute("Guid", Guid.NewGuid().ToString()),
                        new XElement("{http://schemas.microsoft.com/wix/2006/wi}File",
                            new XAttribute("Id", fileid),
                            new XAttribute("Name", name),
                            new XAttribute("DiskId", "1"),
                            new XAttribute("Source", file),
                            new XAttribute("KeyPath", "yes"))));
                    Feature.Add(new XElement("{http://schemas.microsoft.com/wix/2006/wi}ComponentRef",
                        new XAttribute("Id", componentid)));
                }
            }
        }

        static void RemoveOldFiles(XElement directory, string path)
        {
            var removedComponents = (from component in directory.WixElements("Component")
                                     let file = component.WixElement("File")
                                     where file != null && !Files.Contains(path + (string)file.Attribute("Name"))
                                     let componentref = Feature.ElementById((string)component.Attribute("Id"))
                                     select new { component, componentref }).ToList();
            foreach (var removed in removedComponents)
            {
                removed.componentref.Remove();
                removed.component.Remove();
            }
            foreach (XElement subdir in directory.WixElements("Directory"))
                RemoveOldFiles(subdir, path + (string)subdir.Attribute("Name") + @"\");
        }

        public static void RemoveOldFiles()
        {
            RemoveOldFiles(PFiles, "");
        }

        static void RemoveOldFolders(XElement directory, string path)
        {
            var removed = (from subdir in directory.WixElements("Directory")
                           where !Folders.Contains(path + (string)subdir.Attribute("Name"))
                           select subdir).ToList();
            removed.ForEach(subdir => subdir.Remove());
            foreach (XElement subdir in directory.WixElements("Directory"))
                RemoveOldFolders(subdir, path + (string)subdir.Attribute("Name") + @"\");
        }

        public static void RemoveOldFolders()
        {
            RemoveOldFolders(PFiles, "");
        }

        public static void UpdateVersion(string version)
        {
            Product.SetAttributeValue("Version", version);
            Product.WixElement("Upgrade").WixElement("UpgradeVersion").SetAttributeValue("Maximum", version);
        }

        static XElement WixElement(this XElement parent, string name)
        {
            return parent.Element(Ns + name);
        }

        static IEnumerable<XElement> WixElements(this XElement parent, string name)
        {
            return parent.Elements(Ns + name);
        }
    }
}
