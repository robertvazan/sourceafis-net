using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.General
{
    public sealed class Logger
    {
        public delegate bool PathFilter(string path);
        public delegate void Task();

        public static ObjectTree Resolver = new ObjectTree();
        public static PathFilter Filter = delegate(string path) { return false; };

        static Dictionary<string, List<object>> History = new Dictionary<string, List<object>>();

        [ThreadStatic]
        static string ThreadName;

        public static void Clear()
        {
            History.Clear();
        }

        public static T Retrieve<T>(string path)
        {
            return Retrieve<T>(path, 0);
        }

        public static T Retrieve<T>(string path, int index)
        {
            return (T)History[path][index];
        }

        public static void Log(string path, object data)
        {
            if (Filter(path))
            {
                object logged;
                if (data is ICloneable)
                    logged = ((ICloneable)data).Clone();
                else
                    logged = data;
                if (!History.ContainsKey(path))
                    History[path] = new List<object>();
                History[path].Add(logged);
            }
        }

        static string GetThreadName()
        {
            if (ThreadName != null)
                return "[" + ThreadName + "]";
            else
                return "";
        }

        public static void Log(object source, object data)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source) + GetThreadName(), data);
        }

        public static void Log(object source, string part, object data)
        {
            if (Resolver.Contains(source))
                Log(Resolver.GetPath(source) + "." + part + GetThreadName(), data);
        }

        public static void RunInContext(string name, Task task)
        {
            ThreadName = name;
            try
            {
                task();
            }
            finally
            {
                ThreadName = null;
            }
        }
    }
}
