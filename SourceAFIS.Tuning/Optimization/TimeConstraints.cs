using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Tuning.Reports;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class TimeConstraints
    {
        public float Extraction = 0.3f;
        public float Prepare = 0.05f;
        public float Matching = 0.005f;
        public float NonMatching = 0.005f;

        public bool Check(ExtractorReport report)
        {
            return report.Time <= Extraction;
        }

        public bool Check(MatcherReport report)
        {
            return report.Time.Prepare <= Prepare
                && report.Time.Matching <= Matching
                && report.Time.NonMatching <= NonMatching;
        }
    }
}
