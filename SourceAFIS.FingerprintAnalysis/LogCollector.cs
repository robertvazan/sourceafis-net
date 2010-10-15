using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using SourceAFIS.Meta;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.General;
using SourceAFIS.Visualization;
using SourceAFIS.Matching;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogCollector : INotifyPropertyChanged
    {
        public Options Options;
        public ExtractionData Probe = new ExtractionData();
        public ExtractionData Candidate = new ExtractionData();
        public MatchData Match = new MatchData();

        DetailLogger.LogData ProbeLogValue;
        public DetailLogger.LogData ProbeLog
        {
            get { return ProbeLogValue; }
            set { ProbeLogValue = value; OnPropertyChanged("ProbeLog"); }
        }
        
        DetailLogger.LogData CandidateLogValue;
        public DetailLogger.LogData CandidateLog
        {
            get { return CandidateLogValue; }
            set { CandidateLogValue = value; OnPropertyChanged("CandidateLog"); }
        }

        DetailLogger.LogData MatchLogValue;
        public DetailLogger.LogData MatchLog
        {
            get { return MatchLogValue; }
            set { MatchLogValue = value; OnPropertyChanged("MatchLog"); }
        }

        DetailLogger Logger = new DetailLogger();
        Extractor Extractor = new Extractor();
        ParallelMatcher Matcher = new ParallelMatcher();

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public LogCollector()
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(Extractor, "Extractor");
            tree.Scan(Matcher, "Matcher");
            Logger.Attach(tree);

            Probe.SetSource(this, "ProbeLog");
            Candidate.SetSource(this, "CandidateLog");
            Match.SetSource(this, "MatchLog");
        }

        public void Collect()
        {
            ProbeLog = CollectExtraction(Probe, Options.Probe);
            CandidateLog = CollectExtraction(Candidate, Options.Candidate);
            CollectMatching();
        }

        public DetailLogger.LogData CollectExtraction(ExtractionData data, FingerprintOptions fpOptions)
        {
            if (data.InputImage != null)
                Extractor.Extract(data.InputImage, 500);
            return Logger.PopLog();
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
            }
            MatchLog = Logger.PopLog();
        }
    }
}
