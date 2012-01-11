using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class RootPairSelector
    {
        public DetailLogger.Hook Logger = DetailLogger.Null;

        public IEnumerable<MinutiaPair> GetRoots(ProbeIndex probeIndex, Template candidateTemplate)
        {
            var edgeConstructor = new EdgeConstructor();
            for (int candidateReference = 1; candidateReference < candidateTemplate.Minutiae.Length; ++candidateReference)
                for (int candidateNeighbor = 0; candidateNeighbor < candidateTemplate.Minutiae.Length; ++candidateNeighbor)
                {
                    int mixedCandidateReference = (candidateReference + candidateNeighbor) % candidateTemplate.Minutiae.Length;
                    var candidateEdge = edgeConstructor.Construct(candidateTemplate, mixedCandidateReference, candidateNeighbor);
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
