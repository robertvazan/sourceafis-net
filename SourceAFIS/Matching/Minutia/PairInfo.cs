using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Dummy;

namespace SourceAFIS.Matching.Minutia
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
