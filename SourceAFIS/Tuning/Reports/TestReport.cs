using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class TestReport
    {
        public TestConfiguration Configuration = new TestConfiguration();
        public ExtractorReport Extractor;
        public MatcherReport Matcher;

        public void Save(string folder)
        {
            Configuration.Save(folder);
            Extractor.Save(folder);
            Matcher.Save(folder);
        }
    }
}
