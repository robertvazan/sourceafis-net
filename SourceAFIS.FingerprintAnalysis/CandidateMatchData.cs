using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class CandidateMatchData : MatchSideData
    {
        public CandidateMatchData(ExtractionData extraction, MatchData match)
        {
            ExtractionData = extraction;
            Match = match;
        }

        public override List<int> PairedMinutiae
        {
            get
            {
                Link(Match, "Pairing", "PairedMinutiae");
                MinutiaPairing pairing = Match.Pairing;
                return (from index in Enumerable.Range(0, pairing.Count)
                        select pairing.GetPair(index).Candidate).ToList();
            }
        }
    }
}
