using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogDecoder : INotifyPropertyChanged
    {
        public ExtractionData Probe { get; private set; }
        public ExtractionData Candidate { get; private set; }
        public MatchData Match { get; private set; }

        ExtractionCollector ProbeLog;
        ExtractionCollector CandidateLog;
        public MatchCollector MatchLog;

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Initialize(Options options)
        {
            Probe = new ExtractionData();
            Candidate = new ExtractionData();

            Match = new MatchData(this);

            ProbeLog = new ExtractionCollector(options.Probe);
            Probe.Collector = ProbeLog;

            CandidateLog = new ExtractionCollector(options.Candidate);
            Candidate.Collector = CandidateLog;
            
            MatchLog = new MatchCollector(Probe, Candidate);
            Match.Collector = MatchLog;

            NotifyPropertyChanged("Probe");
            NotifyPropertyChanged("Candidate");
            NotifyPropertyChanged("Match");
        }
    }
}
