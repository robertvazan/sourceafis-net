using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SourceAFIS.Meta
{
    public sealed class ObjectTree
    {
        sealed class Item
        {
            public object Reference;
            public string Path;
        }

        Dictionary<string, Item> ByPath = new Dictionary<string,Item>();
        Dictionary<object, Item> ByReference = new Dictionary<object,Item>();

        public ObjectTree()
        {
        }

        public ObjectTree(object root)
        {
            Scan(root);
        }

        public ObjectTree(object root, string path)
        {
            Scan(root, path);
        }

        public void Scan(object root, string path = "")
        {
            if (!ByReference.ContainsKey(root) && !ByPath.ContainsKey(path))
            {
                Item item = new Item();
                item.Reference = root;
                item.Path = path;
                ByPath.Add(path, item);
                ByReference.Add(root, item);

                foreach (FieldInfo fieldInfo in root.GetType().GetFields())
                {
                    if (fieldInfo.GetCustomAttributes(typeof(NestedAttribute), true).Length > 0)
                    {
                        Scan(fieldInfo.GetValue(root), path + (path != "" ? "." : "") + fieldInfo.Name);
                    }
                }
            }
        }

        public void Remove(object reference)
        {
            ByPath.Remove(GetPath(reference));
            ByReference.Remove(reference);
        }

        public void Remove(string path) { Remove(GetObject(path)); }

        public object GetObject(string path) { return ByPath[path].Reference; }
        public string GetPath(object reference) { return ByReference[reference].Path; }
        public bool Contains(string path) { return ByPath.ContainsKey(path); }
        public bool Contains(object reference) { return ByReference.ContainsKey(reference); }
        public object[] GetAllObjects() { return new List<object>(ByReference.Keys).ToArray(); }
        public string[] GetAllPaths() { return new List<string>(ByPath.Keys).ToArray(); }
    }
}
