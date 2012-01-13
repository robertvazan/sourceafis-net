using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Matching.Minutia
{
    public struct EdgePair
    {
        public MinutiaPair Reference;
        public MinutiaPair Neighbor;

        public EdgePair(MinutiaPair reference, MinutiaPair neighbor)
        {
            Reference = reference;
            Neighbor = neighbor;
        }
    }
}
