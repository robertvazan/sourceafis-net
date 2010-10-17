using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogDecoder
    {
        public ExtractionData Probe = new ExtractionData();
        public ExtractionData Candidate = new ExtractionData();
        public MatchData Match = new MatchData();

        public ExtractionCollector ProbeLog { get; set; }
        public ExtractionCollector CandidateLog { get; set; }
        public MatchCollector MatchLog { get; set; }

        public LogDecoder(Options options)
        {
            ProbeLog = new ExtractionCollector(options.Probe);
            Probe.Collector = ProbeLog;

            CandidateLog = new ExtractionCollector(options.Candidate);
            Candidate.Collector = CandidateLog;
            
            MatchLog = new MatchCollector(Probe, Candidate);
            Match.Collector = MatchLog;
        }
    }
}
