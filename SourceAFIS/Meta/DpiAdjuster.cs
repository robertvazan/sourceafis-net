using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SourceAFIS.General;

namespace SourceAFIS.Meta
{
    public sealed class DpiAdjuster
    {
        public int DefaultDpi = 500;

        class Parameter
        {
            public ParameterValue Value;
            public double OriginalValue;
            public DpiAdjustedAttribute Attribute;

            public void Adjust(double factor)
            {
                Value.Value.Double = OriginalValue * factor;
                if (Value.Value.Double < Attribute.Min)
                    Value.Value.Double = Attribute.Min;
                Value.SaveValue();
            }

            public void Revert()
            {
                Value.Value.Double = OriginalValue;
                Value.SaveValue();
            }
        }

        List<Parameter> Parameters = new List<Parameter>();

        public void Initialize(ObjectTree tree)
        {
            Parameters.Clear();
            foreach (string path in tree.GetAllPaths())
            {
                object nested = tree.GetObject(path);
                foreach (FieldInfo field in nested.GetType().GetFields())
                    foreach (object attribute in field.GetCustomAttributes(typeof(DpiAdjustedAttribute), true))
                    {
                        Parameter parameter = new Parameter();
                        parameter.Value = new ParameterValue(path, tree, field);
                        parameter.OriginalValue = parameter.Value.Value.Double;
                        parameter.Attribute = attribute as DpiAdjustedAttribute;
                        Parameters.Add(parameter);
                    }
            }
        }

        public void Adjust(int dpi)
        {
            double factor = dpi / (double)DefaultDpi;
            foreach (Parameter parameter in Parameters)
                parameter.Adjust(factor);
        }

        public void Revert()
        {
            foreach (Parameter parameter in Parameters)
                parameter.Revert();
        }

        public void Adjust(object root, int dpi, Action function)
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(this, "Dpi");
            Initialize(tree);
            try
            {
                Adjust(dpi);
                function();
            }
            finally
            {
                Revert();
            }
        }
    }
}
