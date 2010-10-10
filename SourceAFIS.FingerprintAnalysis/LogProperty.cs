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
        public Func<Options, bool> Filter = options => true;
        public Func<FingerprintOptions, bool> FingerprintFilter = options => true;

        public LogProperty(string log)
        {
            Log = log;
        }
    }
}
