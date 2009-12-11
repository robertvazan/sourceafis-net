using System;
using System.Collections.Generic;
using System.Text;

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

        public static void Log(string path, float[,] image)
        {
            if (Filter(path))
                History[path] = image.Clone();
        }

        public static void Log(object source, float[,] image)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source), image);
        }

        public static void Log(object source, string part, float[,] image)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source) + "." + part, image);
        }
    }
}
