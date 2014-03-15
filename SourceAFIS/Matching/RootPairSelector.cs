using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class RootPairSelector
    {
        const int MinEdgeLength = 58;
        const int MaxEdgeLookups = 1633;

        public static IEnumerable<MinutiaPair> GetRoots(FingerprintMatcher matcher, FingerprintTemplate candidate)
        {
            var hash = new HashLookup(matcher.EdgeHash);
            int counter = 0;
            var filters = new Predicate<EdgeShape>[]
            {
                shape => shape.Length >= MinEdgeLength,
                shape => shape.Length < MinEdgeLength
            };
            foreach (var shapeFilter in filters)
            {
                for (int step = 1; step < candidate.Minutiae.Count; ++step)
                    for (int pass = 0; pass < step + 1; ++pass)
                        for (int candidateReference = pass; candidateReference < candidate.Minutiae.Count; candidateReference += step + 1)
                        {
                            int candidateNeighbor = (candidateReference + step) % candidate.Minutiae.Count;
                            var candidateEdge = EdgeConstructor.Construct(candidate, candidateReference, candidateNeighbor);
                            if (shapeFilter(candidateEdge))
                            {
                                for (var match = hash.Select(candidateEdge); match != null; match = hash.Next())
                                {
                                    var pair = new MinutiaPair(match.Location.Reference, candidateReference);
                                    yield return pair;
                                    ++counter;
                                    if (counter >= MaxEdgeLookups)
                                        yield break;
                                }
                            }
                        }
            }
        }
    }
}
