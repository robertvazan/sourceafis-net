using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Meta
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DpiAdjustedAttribute : Attribute
    {
        double MinValue = 1;
        public double Min
        {
            get { return MinValue; }
            set { MinValue = value; }
        }
    }
}
