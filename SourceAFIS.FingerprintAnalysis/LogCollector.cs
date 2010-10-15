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
            CollectExtraction(Probe, Options.Probe);
            CollectExtraction(Candidate, Options.Candidate);
            CollectMatching();
        }

        public void CollectExtraction(ExtractionData data, FingerprintOptions fpOptions)
        {
            if (data.InputImage != null)
                Extractor.Extract(data.InputImage, 500);
            data.CollectLogs(Logger);
            data.Ridges.CollectLogs(Logger);
            data.Valleys.CollectLogs(Logger);
            Logger.Clear();
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
            }
            Match.CollectLogs(Logger);
            Logger.Clear();
        }
    }
}
