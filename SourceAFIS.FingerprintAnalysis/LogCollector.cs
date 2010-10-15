using System;
using System.Collections.Generic;
using System.Text;
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

        public DetailLogger.LogData ProbeLog;
        public DetailLogger.LogData CandidateLog;
        public DetailLogger.LogData MatchLog;

        DetailLogger Logger = new DetailLogger();
        Extractor Extractor = new Extractor();
        ParallelMatcher Matcher = new ParallelMatcher();

        public LogCollector()
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(Extractor, "Extractor");
            tree.Scan(Matcher, "Matcher");
            Logger.Attach(tree);
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
            DetailLogger.LogData log = Logger.PopLog();
            data.CollectLogs(log);
            data.Ridges.CollectLogs(log);
            data.Valleys.CollectLogs(log);
            return log;
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
            }
            MatchLog = Logger.PopLog();
            Match.CollectLogs(MatchLog);
        }
    }
}
