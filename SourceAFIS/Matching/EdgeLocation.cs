using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Matching
{
    public struct EdgeLocation
    {
        public int Reference;
        public int Neighbor;

        public EdgeLocation(int reference, int neighbor)
        {
            Reference = (int)reference;
            Neighbor = (int)neighbor;
        }
    }
}
