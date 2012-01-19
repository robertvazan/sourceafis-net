using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class RootPairSelector
    {
        [Parameter(Lower = 5, Upper = 300)]
        public int MinEdgeLength = 58;
        [Parameter(Lower = 5, Upper = 100000)]
        public int MaxEdgeLookups = 49271;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        HashLookup HashLookup = new HashLookup();
        int LookupCounter;

        public IEnumerable<MinutiaPair> GetRoots(ProbeIndex probeIndex, Template candidateTemplate)
        {
            LookupCounter = 0;
            HashLookup.Reset(probeIndex.EdgeHash);
            return GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length >= MinEdgeLength)
                .Concat(GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length < MinEdgeLength));
        }

        public IEnumerable<MinutiaPair> GetFilteredRoots(ProbeIndex probeIndex, Template candidateTemplate, Predicate<EdgeShape> shapeFilter)
        {
            if (LookupCounter >= MaxEdgeLookups)
                yield break;
            var edgeConstructor = new EdgeConstructor();
            for (int step = 1; step < candidateTemplate.Minutiae.Length; ++step)
                for (int pass = 0; pass < step + 1; ++pass)
                    for (int candidateReference = pass; candidateReference < candidateTemplate.Minutiae.Length; candidateReference += step + 1)
                    {
                        int candidateNeighbor = (candidateReference + step) % candidateTemplate.Minutiae.Length;
                        var candidateEdge = edgeConstructor.Construct(candidateTemplate, candidateReference, candidateNeighbor);
                        if (shapeFilter(candidateEdge))
                        {
                            for (var match = HashLookup.Select(candidateEdge); match != null; match = HashLookup.Next())
                            {
                                var pair = new MinutiaPair(match.Location.Reference, candidateReference);
                                if (Logger.IsActive)
                                    Logger.Log(pair);
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
