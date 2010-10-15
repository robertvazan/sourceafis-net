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
    public class LogCollector
    {
        public Options Options;

        public ExtractionData Probe = new ExtractionData();
        public ExtractionData Candidate = new ExtractionData();
        public MatchData Match = new MatchData();

        public class CollectorBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected DetailLogger Logger = new DetailLogger();

            DetailLogger.LogData LogsValue;
            public DetailLogger.LogData Logs
            {
                get { return LogsValue; }
                set
                {
                    LogsValue = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Logs"));
                }
            }
        }

        public class ExtractionCollector : CollectorBase
        {
            Extractor Extractor = new Extractor();

            public ExtractionCollector()
            {
                Logger.Attach(new ObjectTree(Extractor));
            }

            public void Collect(byte[,] image)
            {
                if (image != null)
                    Extractor.Extract(image, 500);
                Logs = Logger.PopLog();
            }
        }

        public class MatchCollector : CollectorBase
        {
            ParallelMatcher Matcher = new ParallelMatcher();

            public MatchCollector()
            {
                Logger.Attach(new ObjectTree(Matcher));
            }

            public void Collect(Template probe, Template candidate)
            {
                if (probe != null && candidate != null)
                {
                    ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(probe);
                    Matcher.Match(prepared, new Template[] { candidate });
                }
                Logs = Logger.PopLog();
            }
        }

        public ExtractionCollector ProbeLog { get; set; }
        public ExtractionCollector CandidateLog { get; set; }
        public MatchCollector MatchLog { get; set; }

        public LogCollector()
        {
            Probe.SetSource(ProbeLog, "Logs");
            Candidate.SetSource(CandidateLog, "Logs");
            Match.SetSource(MatchLog, "Logs");
        }

        public void Collect()
        {
            ProbeLog.Collect(Probe.InputImage);
            CandidateLog.Collect(Candidate.InputImage);
            MatchLog.Collect(Probe.Template, Candidate.Template);
        }
    }
}
