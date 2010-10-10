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
            var filter = data.CreateFilter(Options, fpOptions);
            var ridgeFilter = data.Ridges.CreateFilter(Options, fpOptions);
            var valleyFilter = data.Valleys.CreateFilter(Options, fpOptions);
            Logger.Filter = log => filter.Contains(log) || ridgeFilter.Contains(log) || valleyFilter.Contains(log);
            if (data.InputImage != null)
                Extractor.Extract(data.InputImage, 500);
            data.CollectLogs(Logger);
            data.Ridges.CollectLogs(Logger);
            data.Valleys.CollectLogs(Logger);
            Logger.Clear();
        }

        void CollectMatching()
        {
            var filter = Match.CreateFilter(Options);
            Logger.Filter = log => filter.Contains(log);
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
