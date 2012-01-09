using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Meta
{
    public sealed class ParameterValue : ICloneable
    {
        [XmlIgnore]
        public string ObjectPath;
        [XmlIgnore]
        public object ObjectReference;
        [XmlAttribute]
        public string FieldPath;
        [XmlIgnore]
        public FieldInfo Field;
        [XmlIgnore]
        public ParameterAttribute Attribute;

        public struct NumberConverter
        {
            [XmlAttribute]
            public double Double;
            [XmlIgnore]
            public byte Byte { get { return Convert.ToByte(Double); } set { Double = value; } }
            [XmlIgnore]
            public int Int { get { return Convert.ToInt32(Double); } set { Double = value; } }
            [XmlIgnore]
            public float Float { get { return Convert.ToSingle(Double); } set { Double = value; } }

            [XmlIgnore]
            public int Precision;
            public double PrecisionMultiplier { get { return Math.Pow(10, Precision); } }

            [XmlIgnore]
            public int Normalized
            {
                get { return Convert.ToInt32(Double * PrecisionMultiplier); }
                set { Double = value / PrecisionMultiplier; }
            }
        }

        public NumberConverter Value;
        [XmlIgnore]
        public NumberConverter Lower;
        [XmlIgnore]
        public NumberConverter Upper;

        ParameterValue() { }

        public ParameterValue(object instance, FieldInfo field)
        {
            ObjectReference = instance;
            Field = field;
            foreach (object attribute in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                Attribute = attribute as ParameterAttribute;
            ReadAttribute();
            ReadValue();
        }

        public ParameterValue(string objectPath, ObjectTree tree, FieldInfo field)
        {
            ObjectPath = objectPath;
            ObjectReference = tree.GetObject(objectPath);
            FieldPath = objectPath + "." + field.Name;
            Field = field;
            foreach (object attribute in field.GetCustomAttributes(typeof(ParameterAttribute), true))
                Attribute = attribute as ParameterAttribute;
            ReadAttribute();
            ReadValue();
        }

        public void ReadAttribute()
        {
            if (!Attribute.LowerIsDefault)
                Lower.Double = Attribute.Lower;
            else
            {
                if (Field.FieldType == typeof(int))
                    Lower.Double = 1;
                if (Field.FieldType == typeof(float))
                    Lower.Double = 0;
                if (Field.FieldType == typeof(byte))
                    Lower.Double = 0;
            }

            if (!Attribute.UpperIsDefault)
                Upper.Double = Attribute.Upper;
            else
            {
                if (Field.FieldType == typeof(int))
                    Upper.Double = 1000;
                if (Field.FieldType == typeof(float))
                    Upper.Double = 1;
                if (Field.FieldType == typeof(byte))
                    Upper.Double = Angle.PIB;
            }

            int precision = 0;
            if (Field.FieldType == typeof(float))
                precision = Attribute.Precision;
            Lower.Precision = precision;
            Upper.Precision = precision;
            Value.Precision = precision;
        }

        public void ReadValue(object instance)
        {
            if (Field.FieldType == typeof(int))
                Value.Int = (int)Field.GetValue(instance);
            else if (Field.FieldType == typeof(float))
                Value.Float = (float)Field.GetValue(instance);
            else if (Field.FieldType == typeof(byte))
                Value.Byte = (byte)Field.GetValue(instance);
            else
                throw new AssertException();
        }

        public void SaveValue(object instance)
        {
            if (Field.FieldType == typeof(int))
                Field.SetValue(instance, Value.Int);
            else if (Field.FieldType == typeof(float))
                Field.SetValue(instance, Value.Float);
            else if (Field.FieldType == typeof(byte))
                Field.SetValue(instance, Value.Byte);
            else
                throw new AssertException();
        }

        public void ReadValue()
        {
            if (ObjectReference != null)
                ReadValue(ObjectReference);
        }

        public void SaveValue()
        {
            if (ObjectReference != null)
                SaveValue(ObjectReference);
        }

        public void Rebind(object reference)
        {
            ObjectReference = reference;
        }

        public void Rebind(ObjectTree tree)
        {
            if (tree.Contains(ObjectPath))
                Rebind(tree.GetObject(ObjectPath));
        }

        public ParameterValue Clone()
        {
            ParameterValue clone = new ParameterValue();
            clone.ObjectPath = ObjectPath;
            clone.ObjectReference = ObjectReference;
            clone.FieldPath = FieldPath;
            clone.Field = Field;
            clone.Attribute = Attribute;
            clone.Value = Value;
            clone.Lower = Lower;
            clone.Upper = Upper;
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }

        public bool PersistentlyEquals(ParameterValue other)
        {
            return Value.Double == other.Value.Double;
        }
    }
}
