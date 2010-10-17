using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogCollector
    {
        public ExtractionData Probe = new ExtractionData();
        public ExtractionData Candidate = new ExtractionData();
        public MatchData Match = new MatchData();

        public ExtractionCollector ProbeLog { get; set; }
        public ExtractionCollector CandidateLog { get; set; }
        public MatchCollector MatchLog { get; set; }

        public LogCollector(Options options)
        {
            ProbeLog = new ExtractionCollector(options.Probe);
            Probe.SetSource(ProbeLog, "Logs");

            CandidateLog = new ExtractionCollector(options.Candidate);
            Candidate.SetSource(CandidateLog, "Logs");
            
            MatchLog = new MatchCollector(Probe, Candidate);
            Match.SetSource(MatchLog, "Logs");
        }
    }
}
