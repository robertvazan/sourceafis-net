using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.General
{
    public class Logger
    {
        public delegate bool PathFilter(string path);

        public static ObjectTree Resolver = new ObjectTree();
        public static PathFilter Filter = delegate(string path) { return false; };

        static Dictionary<string, object> History = new Dictionary<string, object>();

        public static void Clear()
        {
            History.Clear();
        }

        public static T Retrieve<T>(string path)
        {
            return (T)History[path];
        }

        public static void Log(string path, object data)
        {
            if (Filter(path))
            {
                if (data is ICloneable)
                    History[path] = ((ICloneable)data).Clone();
                else
                    History[path] = data;
            }
        }

        public static void Log(object source, object data)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source), data);
        }

        public static void Log(object source, string part, object data)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source) + "." + part, data);
        }
    }
}
