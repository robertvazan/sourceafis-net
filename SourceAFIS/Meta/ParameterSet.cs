using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using SourceAFIS.General;

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
            T clone = (T)prototype.GetType().GetConstructor(Type.EmptyTypes).Invoke(null);
            ParameterSet parameters = new ParameterSet(new ObjectTree(prototype));
            parameters.Rebind(new ObjectTree(clone));
            parameters.SaveValues();
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
                if (Calc.BeginsWith(parameter.FieldPath, beginsWith))
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
                if (ByPath[path].Value.Double != other.ByPath[path].Value.Double)
                    return false;
            }
            return true;
        }
    }
}
