using System;
using System.Collections.Generic;
using System.Text;

namespace FingerprintAnalyzer
{
    sealed class Options
    {
        public ExtractionOptions Probe = new ExtractionOptions();
        public ExtractionOptions Candidate = new ExtractionOptions();
        public MatchingOptions Match = new MatchingOptions();
    }
}
