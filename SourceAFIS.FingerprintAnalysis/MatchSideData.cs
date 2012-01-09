using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public abstract class MatchSideData : LogData
    {
        protected MatchData Match;
        protected ExtractionData ExtractionData;

        public abstract List<int> PairedMinutiae { get; }

        public List<Point> PairedPoints
        {
            get
            {
                Link(ExtractionData, "Template", "PairedPoints");
                Template template = ExtractionData.Template;
                Link("PairedMinutiae", "PairedPoints");
                var minutiae = PairedMinutiae;
                return (from index in minutiae
                        select template.Minutiae[index].Position.ToPoint()).ToList();
            }
        }
    }
}
