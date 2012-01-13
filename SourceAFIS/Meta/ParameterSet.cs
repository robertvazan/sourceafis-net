using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Meta
{
    public sealed class ParameterSet : ICloneable
    {
        [XmlIgnore]
        Dictionary<string, ParameterValue> ByPath = new Dictionary<string, ParameterValue>();

        public ParameterValue[] AllParameters
        {
            get { return new List<ParameterValue>(ByPath.Values).ToArray(); }
            set
            {
                ByPath.Clear();
                foreach (ParameterValue parameter in value)
                    Add(parameter);
            }
        }

        [XmlIgnore]
        public IEnumerable<ParameterValue> AllTunableParameters { get { return AllParameters.Where(p => !p.Attribute.TuningDisabled); } }

        public ParameterSet()
        {
        }

        public ParameterSet(ObjectTree tree)
        {
            Add(tree);
        }

        public void Add(ObjectTree tree)
        {
            foreach (string objectPath in tree.GetAllPaths())
            {
                object objectReference = tree.GetObject(objectPath);
                foreach (FieldInfo field in objectReference.GetType().GetFields())
                    foreach (object attribute in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                    {
                        ParameterValue parameter = new ParameterValue(objectPath, tree, field);
                        ByPath[parameter.FieldPath] = parameter;
                    }
            }
        }

        public void Add(ParameterValue parameter)
        {
            ByPath[parameter.FieldPath] = parameter;
        }

        public ParameterValue Get(string path)
        {
            return ByPath[path];
        }

        public bool Contains(string path)
        {
            return ByPath.ContainsKey(path);
        }

        public void Rebind(ObjectTree tree)
        {
            foreach (ParameterValue parameter in ByPath.Values)
                parameter.Rebind(tree);
        }

        public void ReadValues()
        {
            foreach (ParameterValue parameter in ByPath.Values)
                parameter.ReadValue();
        }

        public void SaveValues()
        {
            foreach (ParameterValue parameter in ByPath.Values)
                parameter.SaveValue();
        }

        public static T ClonePrototype<T>(T prototype) where T : class
        {
            T clone = prototype.GetType().GetConstructor(Type.EmptyTypes).Invoke(null) as T;
            ParameterSet parameters = new ParameterSet(new ObjectTree(prototype));
            parameters.Rebind(new ObjectTree(clone));
            parameters.SaveValues();
            DetailLogger.CopyHooks(prototype, clone);
            return clone;
        }

        public ParameterSet Clone()
        {
            ParameterSet clone = new ParameterSet();
            foreach (ParameterValue parameter in ByPath.Values)
                clone.Add(parameter.Clone());
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }

        public ParameterSet GetSubset(string beginsWith)
        {
            ParameterSet result = new ParameterSet();
            foreach (ParameterValue parameter in ByPath.Values)
                if (parameter.FieldPath.BeginsWith(beginsWith))
                    result.Add(parameter.Clone());
            return result;
        }

        public bool PersistentlyEquals(ParameterSet other)
        {
            if (ByPath.Count != other.ByPath.Count)
                return false;
            foreach (string path in ByPath.Keys)
            {
                if (!other.ByPath.ContainsKey(path))
                    return false;
                if (!ByPath[path].PersistentlyEquals(other.ByPath[path]))
                    return false;
            }
            return true;
        }

        public ParameterSet GetDifferences(ParameterSet original)
        {
            ParameterSet diff = new ParameterSet();
            foreach (ParameterValue parameter in ByPath.Values)
                if (!original.Contains(parameter.FieldPath) || !parameter.PersistentlyEquals(original.Get(parameter.FieldPath)))
                    diff.Add(parameter.Clone());
            return diff;
        }

        public ParameterValue GetDifference(ParameterSet original)
        {
            ParameterSet diff = GetDifferences(original);
            ParameterValue[] all = diff.AllParameters;
            AssertException.Check(all.Length <= 1);
            return all[0];
        }
    }
}
