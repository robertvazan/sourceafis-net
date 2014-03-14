using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class RootPairSelector
    {
        const int MinEdgeLength = 58;
        const int MaxEdgeLookups = 1633;

        HashLookup HashLookup = new HashLookup();
        int LookupCounter;

        public IEnumerable<MinutiaPair> GetRoots(ProbeIndex probeIndex, FingerprintTemplate candidateTemplate)
        {
            LookupCounter = 0;
            HashLookup.Reset(probeIndex.EdgeHash);
            return GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length >= MinEdgeLength)
                .Concat(GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length < MinEdgeLength));
        }

        public IEnumerable<MinutiaPair> GetFilteredRoots(ProbeIndex probeIndex, FingerprintTemplate candidateTemplate, Predicate<EdgeShape> shapeFilter)
        {
            if (LookupCounter >= MaxEdgeLookups)
                yield break;
            for (int step = 1; step < candidateTemplate.Minutiae.Count; ++step)
                for (int pass = 0; pass < step + 1; ++pass)
                    for (int candidateReference = pass; candidateReference < candidateTemplate.Minutiae.Count; candidateReference += step + 1)
                    {
                        int candidateNeighbor = (candidateReference + step) % candidateTemplate.Minutiae.Count;
                        var candidateEdge = EdgeConstructor.Construct(candidateTemplate, candidateReference, candidateNeighbor);
                        if (shapeFilter(candidateEdge))
                        {
                            for (var match = HashLookup.Select(candidateEdge); match != null; match = HashLookup.Next())
                            {
                                var pair = new MinutiaPair(match.Location.Reference, candidateReference);
                                yield return pair;
                                ++LookupCounter;
                                if (LookupCounter >= MaxEdgeLookups)
                                    yield break;
                            }
                        }
                    }
        }
    }
}
