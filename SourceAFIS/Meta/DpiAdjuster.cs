using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SourceAFIS.Meta
{
    public class DpiAdjuster
    {
        public delegate void VoidFunction();

        public int DefaultDpi = 500;

        class Parameter
        {
            public ParameterValue Value;
            public double OriginalValue;
            public DpiAdjustedAttribute Attribute;

            public void Adjust(double factor)
            {
                Value.DoubleValue = OriginalValue * factor;
                if (Value.DoubleValue < Attribute.Min)
                    Value.DoubleValue = Attribute.Min;
                Value.SaveValue();
            }

            public void Revert()
            {
                Value.DoubleValue = OriginalValue;
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
                        parameter.OriginalValue = parameter.Value.DoubleValue;
                        parameter.Attribute = (DpiAdjustedAttribute)attribute;
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

        public void Adjust(object root, int dpi, VoidFunction function)
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
