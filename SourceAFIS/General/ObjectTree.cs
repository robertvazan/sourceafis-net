using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SourceAFIS.General
{
    public class ObjectTree
    {
        class Item
        {
            public object Reference;
            public string Path;
        }

        Dictionary<string, Item> ByPath = new Dictionary<string,Item>();
        Dictionary<object, Item> ByReference = new Dictionary<object,Item>();

        public void Scan(object root, string path)
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
                        Scan(fieldInfo.GetValue(root), path + "." + fieldInfo.Name);
                    }
                }
            }
        }

        public object GetObject(string path) { return ByPath[path].Reference; }
        public string GetPath(object reference) { return ByReference[reference].Path; }
        public bool Contains(string path) { return ByPath.ContainsKey(path); }
        public bool Contains(object reference) { return ByReference.ContainsKey(reference); }
    }
}
