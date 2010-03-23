using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class MutationAdvice
    {
        public ParameterSet Initial;
        public ParameterSet Mutated;
        public float Confidence;
    }
}
