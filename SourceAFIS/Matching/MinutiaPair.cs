using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Matching
{
    public struct MinutiaPair
    {
        public short Probe;
        public short Candidate;

        public MinutiaPair(int probe, int candidate)
        {
            Probe = (short)probe;
            Candidate = (short)candidate;
        }
    }
}
