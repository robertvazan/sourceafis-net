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

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public IEnumerable<MinutiaPair> GetRoots(ProbeIndex probeIndex, Template candidateTemplate)
        {
            return GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length >= MinEdgeLength)
                .Concat(GetFilteredRoots(probeIndex, candidateTemplate, shape => shape.Length < MinEdgeLength));
        }

        public IEnumerable<MinutiaPair> GetFilteredRoots(ProbeIndex probeIndex, Template candidateTemplate, Predicate<EdgeShape> shapeFilter)
        {
            var edgeConstructor = new EdgeConstructor();
            for (int candidateReference = 1; candidateReference < candidateTemplate.Minutiae.Length; ++candidateReference)
                for (int candidateNeighbor = 0; candidateNeighbor < candidateTemplate.Minutiae.Length; ++candidateNeighbor)
                {
                    int mixedCandidateReference = (candidateReference + candidateNeighbor) % candidateTemplate.Minutiae.Length;
                    var candidateEdge = edgeConstructor.Construct(candidateTemplate, mixedCandidateReference, candidateNeighbor);
                    if (shapeFilter(candidateEdge))
                    {
                        foreach (var probeEdge in probeIndex.EdgeHash.FindMatching(candidateEdge))
                        {
                            var pair = new MinutiaPair(probeEdge.Reference, mixedCandidateReference);
                            if (Logger.IsActive)
                                Logger.Log(pair);
                            yield return pair;
                        }
                    }
                }
        }
    }
}
