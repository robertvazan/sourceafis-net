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

        public void Check(ExtractorReport report)
        {
            if (report.Time > Extraction)
                throw new FailedMutationException("Extraction time above limit: {0}ms", Convert.ToInt32(report.Time * 1000));
        }

        public void Check(MatcherReport report)
        {
            if (report.Time.Prepare > Prepare)
                throw new FailedMutationException("Probe indexing time above limit: {0}ms", Convert.ToInt32(report.Time.Prepare * 1000));
            if (report.Time.Matching > Matching)
                throw new FailedMutationException("Matching pair time above limit: {0}us", Convert.ToInt32(report.Time.Matching * 1000000));
            if (report.Time.NonMatching > NonMatching)
                throw new FailedMutationException("Non-matching pair time above limit: {0}us", Convert.ToInt32(report.Time.NonMatching * 1000000));
        }
    }
}
