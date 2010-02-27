using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Meta
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ParameterAttribute : Attribute
    {
        // Defaults:
        // Int32: Lower = 1, Upper = 1000
        // Single: Lower = 0, Upper = 1, Precision = 2
        // Byte (angle): Lower = 0, Upper = Angle.PIB

        bool LowerIsDefaultValue = true;
        double LowerValue;
        public double Lower
        {
            get { return LowerValue; }
            set { LowerValue = value; LowerIsDefaultValue = false; }
        }
        public bool LowerIsDefault { get { return LowerIsDefaultValue; } }

        bool UpperIsDefaultValue = true;
        double UpperValue;
        public double Upper
        {
            get { return UpperValue; }
            set { UpperValue = value; UpperIsDefaultValue = false; }
        }
        public bool UpperIsDefault { get { return UpperIsDefaultValue; } }

        int PrecisionValue = 2;
        public int Precision
        {
            get { return PrecisionValue; }
            set { PrecisionValue = value; }
        }
    }
}
