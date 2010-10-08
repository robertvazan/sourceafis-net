using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class MatchData
    {
        public float Score;
        public bool AnyMatch;
        public MinutiaPair Root;
        public MinutiaPairing Pairing;
    }
}
