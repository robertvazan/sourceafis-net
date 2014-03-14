using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SourceAFIS.General;
using SourceAFIS.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class ParallelMatcher
    {
        public MinutiaMatcher MinutiaMatcher = new MinutiaMatcher();

        public ProbeIndex Prepare(FingerprintTemplate probe)
        {
            var index = new ProbeIndex();
            MinutiaMatcher.BuildIndex(probe, index);
            return index;
        }

        public float[] Match(ProbeIndex probe, IList<FingerprintTemplate> candidates)
        {
            MinutiaMatcher.SelectProbe(probe);
            return candidates.Select(c => MinutiaMatcher.Match(c)).ToArray();
        }
    }
}
