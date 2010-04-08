using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class ExhaustiveRootSelector : IRootSelector
    {
        public IEnumerable<MinutiaPair> GetRoots(Template probeTemplate, Template candidateTemplate)
        {
            for (int probe = 0; probe < probeTemplate.Minutiae.Length; ++probe)
                for (int candidate = 0; candidate < candidateTemplate.Minutiae.Length; ++candidate)
                    yield return new MinutiaPair(probe, candidate);
        }
    }
}
