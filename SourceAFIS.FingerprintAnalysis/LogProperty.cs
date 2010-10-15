using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogProperty
    {
        public string Name { get; set; }
        public string Log { get; set; }
        public object Value { get; set; }
        public List<ComputedProperty> DependentProperties = new List<ComputedProperty>();

        public LogProperty(string log)
        {
            Log = log;
        }
    }
}
