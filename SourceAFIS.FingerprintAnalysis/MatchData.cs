using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class MatchData : LogData
    {
        public float Score { get { return (float)GetLog("Score", "MinutiaScore"); } }

        public bool AnyMatch { get { Watch("Score", "AnyMatch"); return Score > 0; } }

        public MinutiaPair? Root { get { return (MinutiaPair?)GetLog("Root", "MinutiaRoot"); } }

        public MinutiaPairing Pairing { get { return (MinutiaPairing)GetLog("Pairing", "MinutiaPairing"); } }
    }
}
