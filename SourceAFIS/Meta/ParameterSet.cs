using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SourceAFIS.Meta
{
    public sealed class ParameterSet
    {
        Dictionary<string, ParameterValue> ByPath = new Dictionary<string, ParameterValue>();

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
                    foreach (object attribute in field.GetCustomAttributes(typeof(ParameterAttributes), true))
                    {
                        ParameterValue parameter = new ParameterValue(objectPath, tree, field);
                        parameter.Attribute = (ParameterAttribute)attribute;
                        ByPath[parameter.FieldPath] = parameter;
                    }
            }
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
    }
}
