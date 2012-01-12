using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class RootPairSelector
    {
        [Parameter(Lower = 5, Upper = 300)]
        public int MinEdgeLength = 50;
        [Parameter(Lower = 5, Upper = 100000)]
        public int MaxEdgeLookups = 1000;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        int LookupCounter;

        public IEnumerable<MinutiaPair> GetRoots(ProbeIndex probeIndex, Template candidateTemplate)
        {
            LookupCounter = 0;
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
                            foreach (var probeEdge in probeIndex.EdgeHash.FindMatching(candidateEdge))
                            {
                                var pair = new MinutiaPair(probeEdge.Reference, candidateReference);
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
