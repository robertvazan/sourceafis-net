using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class MatchData : LogData
    {
        public MatchData(LogDecoder logs)
        {
            Probe = new ProbeMatchData(logs.Probe, this);
            Candidate = new CandidateMatchData(logs.Candidate, this);
        }

        public ProbeMatchData Probe;
        public CandidateMatchData Candidate;

        public float Score { get { return (float)GetLog("Score", "MinutiaMatcher.Score"); } }

        public bool AnyMatch { get { Link("Score", "AnyMatch"); return Score > 0; } }

        public MinutiaPair? Root { get { return (MinutiaPair?)GetLog("Root", "MinutiaMatcher.Root"); } }

        public MinutiaPairing Pairing { get { return (MinutiaPairing)GetLog("Pairing", "MinutiaMatcher.Pairing"); } }
    }
}
