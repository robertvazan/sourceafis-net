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
            Filter = options => options.Probe.Path != "" && options.Candidate.Path != "";
        }

        public LogProperty ScoreProperty = new LogProperty("Matcher.MinutiaMatcher.Score");
        public float Score { get { return (float?)ScoreProperty.Value ?? 0; } }

        public ComputedProperty AnyMatchProperty = new ComputedProperty("Score");
        public bool AnyMatch { get { return Score > 0; } }

        public LogProperty RootProperty = new LogProperty("Matcher.MinutiaMatcher.Root")
        {
            Filter = options => false
        };
        public MinutiaPair? Root { get { return (MinutiaPair?)RootProperty.Value; } }

        public LogProperty PairingProperty = new LogProperty("Matcher.MinutiaMatcher.Pairing");
        public MinutiaPairing Pairing { get { return (MinutiaPairing)PairingProperty.Value; } }
    }
}
