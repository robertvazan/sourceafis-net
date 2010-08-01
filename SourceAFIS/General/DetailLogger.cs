using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SourceAFIS.Meta;

namespace SourceAFIS.General
{
    public sealed class DetailLogger
    {
        public abstract class Hook
        {
            public abstract void Log(object data);
            public abstract void Log(string part, object data);
        }

        class NullHook : Hook
        {
            public override void Log(object data) { }
            public override void Log(string part, object data) { }
        }

        class ActiveHook : Hook, ICloneable
        {
            public DetailLogger Logger;
            public string Path;

            public ActiveHook(DetailLogger logger, string path)
            {
                Logger = logger;
                Path = path;
            }

            public override void Log(object data)
            {
                Logger.Log(Path, data);
            }

            public override void Log(string part, object data)
            {
                Logger.Log(Path + "." + part, data);
            }

            public ActiveHook Clone() { return new ActiveHook(Logger, Path); }
            object ICloneable.Clone() { return Clone(); }
        }

        public static readonly Hook Null = new NullHook();

        [ThreadStatic]
        static string ThreadName;

        Dictionary<string, List<object>> History = new Dictionary<string, List<object>>();

        public void Attach(ObjectTree tree)
        {
            foreach (object reference in tree.GetAllObjects())
            {
                FieldInfo field = reference.GetType().GetField("Logger");
                if (field != null)
                    field.SetValue(reference, new ActiveHook(this, tree.GetPath(reference)));
            }
        }

        public void Clear()
        {
            lock (this)
                History.Clear();
        }

        public T Retrieve<T>(string path)
        {
            lock (this)
                return Retrieve<T>(path, 0);
        }

        public T Retrieve<T>(string path, int index)
        {
            lock (this)
                return (T)History[path][index];
        }

        public void Log(string path, object data)
        {
            lock (this)
            {
                object logged;
                if (data is ICloneable)
                    logged = ((ICloneable)data).Clone();
                else
                    logged = data;
                string qualifiedPath = path + GetThreadName();
                if (!History.ContainsKey(qualifiedPath))
                    History[qualifiedPath] = new List<object>();
                History[qualifiedPath].Add(logged);
            }
        }

        static string GetThreadName()
        {
            if (ThreadName != null)
                return "[" + ThreadName + "]";
            else
                return "";
        }

        public static void RunInContext(string name, Action task)
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

        public static void CopyHooks(object original, object copy)
        {
            ObjectTree originalTree = new ObjectTree(original);
            ObjectTree copyTree = new ObjectTree(copy);
            foreach (object originalReference in originalTree.GetAllObjects())
            {
                FieldInfo field = originalReference.GetType().GetField("Logger");
                if (field != null)
                {
                    object hook = field.GetValue(originalReference);
                    if (hook is ActiveHook)
                    {
                        string path = originalTree.GetPath(originalReference);
                        if (copyTree.Contains(path))
                        {
                            object copyReference = copyTree.GetObject(path);
                            field.SetValue(copyReference, ((ActiveHook)hook).Clone());
                        }
                    }
                }
            }
        }
    }
}
