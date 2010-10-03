using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public class EnumDescriptionAttribute : Attribute
    {
        public static string[] GetEnumDescriptions(string name)
        {
            return Enum.GetNames(Type.GetType(name));
        }
    }
}
