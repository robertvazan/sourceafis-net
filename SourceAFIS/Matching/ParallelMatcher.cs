using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class ParallelMatcher
    {
        [Nested]
        public MinutiaMatcher MinutiaMatcher = new MinutiaMatcher();

        ProbeIndex ProbeIndex;
        Queue<MinutiaMatcher> Matchers = new Queue<MinutiaMatcher>();

        MinutiaMatcher DequeueMatcher()
        {
            MinutiaMatcher matcher = null;
            lock (Matchers)
                if (Matchers.Count > 0)
                    matcher = Matchers.Dequeue();
            if (matcher == null)
                matcher = ParameterSet.ClonePrototype(MinutiaMatcher);
            if (ProbeIndex != null)
                matcher.SelectProbe(ProbeIndex);
            return matcher;
        }

        void EnqueueMatcher(MinutiaMatcher matcher)
        {
            lock (Matchers)
                Matchers.Enqueue(matcher);
        }

        public void Prepare(Template probe)
        {
            MinutiaMatcher matcher = DequeueMatcher();
            ProbeIndex = new ProbeIndex();
            try
            {
                matcher.BuildIndex(probe, ProbeIndex);
            }
            finally
            {
                EnqueueMatcher(matcher);
            }
        }

        public float[] Match(IList<Template> candidates)
        {
            float[] scores = new float[candidates.Count];
            
            Parallel.For(0, candidates.Count,
                () => DequeueMatcher(),
                (i, state, matcher) => { scores[i] = matcher.Match(candidates[i]); return matcher; },
                (matcher) => { EnqueueMatcher(matcher); });

            return scores;
        }
    }
}
