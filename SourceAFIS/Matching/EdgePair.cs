using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Matching
{
    public struct EdgePair
    {
        public readonly MinutiaPair Reference;
        public readonly MinutiaPair Neighbor;

        public EdgePair(MinutiaPair reference, MinutiaPair neighbor)
        {
            Reference = reference;
            Neighbor = neighbor;
        }
    }
}
