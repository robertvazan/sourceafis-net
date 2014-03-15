using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Matching
{
    public sealed class PairInfo : ICloneable
    {
        public MinutiaPair Pair;
        public MinutiaPair Reference;
        public int SupportingEdges;

        public object Clone()
        {
            return new PairInfo()
            {
                Pair = this.Pair,
                SupportingEdges = this.SupportingEdges,
                Reference = this.Reference
            };
        }
    }
}
