using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.Matching;
using SourceAFIS.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public class MatchCollector : LogCollector
    {
        ParallelMatcher Matcher = new ParallelMatcher();

        public MatchCollector(ExtractionData probe, ExtractionData candidate)
        {
            var tree = new ObjectTree(Matcher);
            tree.Remove("MinutiaMatcher.EdgeTablePrototype");
            tree.Remove("MinutiaMatcher.RootSelector");
            tree.Remove("MinutiaMatcher.Pairing");
            tree.Remove("MinutiaMatcher.MatchScoring");
            Logger.Attach(tree);

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
}
