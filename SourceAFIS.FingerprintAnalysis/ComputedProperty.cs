using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ComputedProperty
    {
        List<string> Dependencies = new List<string>();

        public ComputedProperty(params string[] dependencies)
        {
            Dependencies.AddRange(dependencies);
        }
    }
}
