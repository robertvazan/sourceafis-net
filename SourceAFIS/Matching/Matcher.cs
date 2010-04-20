using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class Matcher
    {
        [Nested]
        public MinutiaMatcher MinutiaMatcher = new MinutiaMatcher();

        ProbeIndex ProbeIndex;
        MinutiaMatcher[] Matchers;

        public void Initialize()
        {
            Matchers = new MinutiaMatcher[Threader.HwThreadCount];
            for (int i = 0; i < Matchers.Length; ++i)
                Matchers[i] = ParameterSet.ClonePrototype(MinutiaMatcher);
        }

        public void Prepare(Template probe)
        {
            ProbeIndex = new ProbeIndex();
            Matchers[0].BuildIndex(probe, ProbeIndex);
            for (int i = 0; i < Matchers.Length; ++i)
                Matchers[i].SelectProbe(ProbeIndex);
        }

        public float[] Match(IList<Template> candidates)
        {
            float[] scores = new float[candidates.Count];
            
            Action<Range>[] rangeMatchers = new Action<Range>[Matchers.Length];
            for (int i = 0; i < rangeMatchers.Length; ++i)
            {
                MinutiaMatcher matcher = Matchers[i];
                rangeMatchers[i] = delegate(Range subrange)
                {
                    for (int j = subrange.Begin; j < subrange.End; ++j)
                        scores[j] = matcher.Match(candidates[j]);
                };
            }

            Threader.Split(new Range(candidates.Count), rangeMatchers);

            return scores;
        }
    }
}
