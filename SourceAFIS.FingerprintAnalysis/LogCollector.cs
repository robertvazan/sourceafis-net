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
                set { LogsValue = value; OnPropertyChanged("Logs"); }
            }

            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public class ExtractionCollector : CollectorBase
        {
            Extractor Extractor = new Extractor();

            byte[,] InputImageValue;
            public byte[,] InputImage
            {
                get { return InputImageValue; }
                set { InputImageValue = value; OnPropertyChanged("InputImage"); }
            }

            public ExtractionCollector(FingerprintOptions options)
            {
                Logger.Attach(new ObjectTree(Extractor));
                Collect(options);
                options.PropertyChanged += OnOptionsChange;
            }

            void OnOptionsChange(object source, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "Path")
                    Collect(source as FingerprintOptions);
            }

            void Collect(FingerprintOptions options)
            {
                InputImage = options.Path != "" ? ImageIO.GetPixels(ImageIO.Load(options.Path)) : null;

                if (InputImage != null)
                    Extractor.Extract(InputImage, 500);
                Logs = Logger.PopLog();
            }
        }

        public class MatchCollector : CollectorBase
        {
            ParallelMatcher Matcher = new ParallelMatcher();

            public MatchCollector(ExtractionData probe, ExtractionData candidate)
            {
                Logger.Attach(new ObjectTree(Matcher));
                Collect(probe.Template, candidate.Template);
                probe.PropertyChanged += (source, args) =>
                {
                    if (args.PropertyName == "Template")
                        Collect(probe.Template, candidate.Template);
                };
                candidate.PropertyChanged += (source, args) =>
                {
                    if (args.PropertyName == "Template")
                        Collect(probe.Template, candidate.Template);
                };
            }

            void Collect(Template probe, Template candidate)
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

        public LogCollector(Options options)
        {
            ProbeLog = new ExtractionCollector(options.Probe);
            CandidateLog = new ExtractionCollector(options.Candidate);
            MatchLog = new MatchCollector(Probe, Candidate);

            Probe.SetSource(ProbeLog, "Logs");
            Candidate.SetSource(CandidateLog, "Logs");
            Match.SetSource(MatchLog, "Logs");
        }
    }
}
