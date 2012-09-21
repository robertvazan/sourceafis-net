using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class ParallelMatcher
    {
        [Nested]
        public MinutiaMatcher MinutiaMatcher = new MinutiaMatcher();

        public class PreparedProbe
        {
            public ProbeIndex ProbeIndex = new ProbeIndex();
        }

        Queue<MinutiaMatcher> Matchers = new Queue<MinutiaMatcher>();

        MinutiaMatcher DequeueMatcher()
        {
            MinutiaMatcher matcher = null;
            lock (Matchers)
                if (Matchers.Count > 0)
                    matcher = Matchers.Dequeue();
            if (matcher == null)
                matcher = ParameterSet.ClonePrototype(MinutiaMatcher);
            return matcher;
        }

        MinutiaMatcher DequeueMatcher(PreparedProbe probe)
        {
            MinutiaMatcher matcher = DequeueMatcher();
            matcher.SelectProbe(probe.ProbeIndex);
            return matcher;
        }

        void EnqueueMatcher(MinutiaMatcher matcher)
        {
            lock (Matchers)
                Matchers.Enqueue(matcher);
        }

        public PreparedProbe Prepare(Template probe)
        {
            PreparedProbe prepared = new PreparedProbe();
            MinutiaMatcher matcher = DequeueMatcher();
            try
            {
                matcher.BuildIndex(probe, prepared.ProbeIndex);
            }
            finally
            {
                EnqueueMatcher(matcher);
            }
            return prepared;
        }

        public float[] Match(PreparedProbe probe, IList<Template> candidates)
        {
            float[] scores = new float[candidates.Count];
            
            Parallel.For(0, candidates.Count,
                () => DequeueMatcher(probe),
                (i, state, matcher) => { scores[i] = matcher.Match(candidates[i]); return matcher; },
                (matcher) => { EnqueueMatcher(matcher); });

            return scores;
        }
    }
}
