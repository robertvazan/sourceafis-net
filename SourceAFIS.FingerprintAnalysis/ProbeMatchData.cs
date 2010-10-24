using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ProbeMatchData : MatchSideData
    {
        public ProbeMatchData(ExtractionData extraction, MatchData match)
        {
            ExtractionData = extraction;
            Match = match;
        }

        public override IEnumerable<int> PairedMinutiae
        {
            get
            {
                Link(Match, "Pairing", "PairedMinutiae");
                MinutiaPairing pairing = Match.Pairing;
                return from index in Enumerable.Range(0, pairing.Count)
                       select pairing.GetPair(index).Probe;
            }
        }
    }
}
