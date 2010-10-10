using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ComputedProperty
    {
        public string Name { get; set; }

        public void AddDependency(LogProperty dependency)
        {
            dependency.DependentProperties.Add(this);
        }
    }
}
