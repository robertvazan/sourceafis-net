using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class MatchData : LogData
    {
        public MatchData()
        {
            RegisterProperties();
        }

        LogProperty ScoreProperty = new LogProperty();
        public float Score { get { return (float)ScoreProperty.Value; } }

        LogProperty AnyMatchProperty = new LogProperty();
        public bool AnyMatch { get { return (bool)AnyMatchProperty.Value; } }

        LogProperty RootProperty = new LogProperty();
        public MinutiaPair Root { get { return (MinutiaPair)RootProperty.Value; } }

        LogProperty PairingProperty = new LogProperty();
        public MinutiaPairing Pairing { get { return (MinutiaPairing)PairingProperty.Value; } }
    }
}
