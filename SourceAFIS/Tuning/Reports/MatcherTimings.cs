using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class MatcherTimings
    {
        public float Prepare;
        public float Matching;
        public float NonMatching;
        public float Average { get { return (Matching + NonMatching) / 2; } }
    }
}
