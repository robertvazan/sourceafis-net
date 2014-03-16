using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Matching
{
    public struct MinutiaPair
    {
        public int Probe;
        public int Candidate;

        public MinutiaPair(int probe, int candidate)
        {
            Probe = probe;
            Candidate = candidate;
        }
    }
}
