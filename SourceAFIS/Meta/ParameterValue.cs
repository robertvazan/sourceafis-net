using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SourceAFIS.Meta
{
    public class ParameterValue
    {
        public string ObjectPath;
        public object ObjectReference;
        public string FieldPath;
        public FieldInfo Field;

        public double DoubleValue;
        public int IntValue { get { return Convert.ToInt32(DoubleValue); } set { DoubleValue = value; } }
        public float FloatValue { get { return Convert.ToSingle(DoubleValue); } set { DoubleValue = value; } }

        public ParameterValue(object instance, FieldInfo field)
        {
            ObjectReference = instance;
            Field = field;
            ReadValue();
        }

        public ParameterValue(string objectPath, ObjectTree tree, FieldInfo field)
        {
            ObjectPath = objectPath;
            ObjectReference = tree.GetObject(objectPath);
            FieldPath = objectPath + "." + field.Name;
            Field = field;
            ReadValue();
        }

        public void ReadValue(object instance)
        {
            if (Field.FieldType == typeof(int))
                IntValue = (int)Field.GetValue(instance);
            else if (Field.FieldType == typeof(float))
                FloatValue = (float)Field.GetValue(instance);
            else
                throw new Exception();
        }

        public void SaveValue(object instance)
        {
            if (Field.FieldType == typeof(int))
                Field.SetValue(instance, IntValue);
            else if (Field.FieldType == typeof(float))
                Field.SetValue(instance, FloatValue);
            else
                throw new Exception();
        }

        public void ReadValue() { ReadValue(ObjectReference); }
        public void SaveValue() { SaveValue(ObjectReference); }
    }
}
