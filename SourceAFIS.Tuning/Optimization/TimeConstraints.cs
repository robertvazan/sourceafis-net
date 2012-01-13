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
        public float NonMatching = 0.000095f;

        public bool Check(ExtractorReport report, bool tolerant)
        {
            var tolerance = tolerant ? 1.1 : 1;
            return report.Time <= Extraction * tolerance;
        }

        public bool Check(MatcherReport report, bool tolerant)
        {
            var tolerance = tolerant ? 1.1 : 1;
            return report.Time.Prepare <= Prepare * tolerance
                && report.Time.Matching <= Matching * tolerance
                && report.Time.NonMatching <= NonMatching * tolerance;
        }
    }
}
