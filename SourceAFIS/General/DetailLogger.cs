using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SourceAFIS.Meta;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public sealed class DetailLogger
    {
        public abstract class Hook
        {
            public abstract bool IsActive { get; }
            public abstract void Log(object data);
            public abstract void Log(string part, object data);
        }

        class NullHook : Hook
        {
            public override bool IsActive { get { return false; } }
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

            public override bool IsActive { get { return true; } }

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

        public class LogData
        {
            Dictionary<string, List<object>> History = new Dictionary<string, List<object>>();

            public void Append(string path, object data)
            {
                if (!History.ContainsKey(path))
                    History[path] = new List<object>();
                History[path].Add(data);
            }

            public object Retrieve(string path)
            {
                lock (this)
                    return Retrieve(path, 0);
            }

            public object Retrieve(string path, int index)
            {
                lock (this)
                {
                    if (History.ContainsKey(path) && index < History[path].Count)
                        return History[path][index];
                    else
                        return null;
                }
            }
        }

        public static readonly Hook Null = new NullHook();

        [ThreadStatic]
        static string ThreadName;

        LogData CurrentLog = new LogData();

        public LogData PopLog()
        {
            LogData result = CurrentLog;
            CurrentLog = new LogData();
            return result;
        }

        public void Attach(ObjectTree tree)
        {
            foreach (object reference in tree.GetAllObjects())
            {
                FieldInfo field = reference.GetType().GetField("Logger");
                if (field != null)
                    field.SetValue(reference, new ActiveHook(this, tree.GetPath(reference)));
            }
        }

        public void Log(string path, object data)
        {
            lock (this)
            {
                object logged;
                if (data is ICloneable)
                    logged = (data as ICloneable).Clone();
                else
                    logged = data;
                string qualifiedPath = path + GetThreadName();
                CurrentLog.Append(qualifiedPath, logged);
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
                            field.SetValue(copyReference, (hook as ActiveHook).Clone());
                        }
                    }
                }
            }
        }
    }
}
