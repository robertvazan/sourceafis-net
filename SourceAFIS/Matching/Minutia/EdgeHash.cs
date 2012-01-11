using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class EdgeHash
    {
        class IndexedEdge
        {
            public EdgeInfo Shape;
            public EdgeLocation Location;
        }

        EdgeLookup EdgeLookup;
        Dictionary<int, object> Hash = new Dictionary<int, object>();

        public EdgeHash(Template template, EdgeLookup lookup)
        {
            EdgeLookup = lookup;
            var edgeConstructor = new EdgeConstructor();
            for (int referenceMinutia = 0; referenceMinutia < template.Minutiae.Length; ++referenceMinutia)
                for (int neighborMinutia = 0; neighborMinutia < template.Minutiae.Length; ++neighborMinutia)
                    if (referenceMinutia != neighborMinutia)
                    {
                        var edge = new IndexedEdge()
                        {
                            Shape = edgeConstructor.Construct(template, referenceMinutia, neighborMinutia),
                            Location = new EdgeLocation(referenceMinutia, neighborMinutia)
                        };
                        foreach (var hash in lookup.HashCoverage(edge.Shape))
                        {
                            object value;
                            List<IndexedEdge> list;
                            if (!Hash.TryGetValue(hash, out value))
                                Hash[hash] = edge;
                            else
                            {
                                list = value as List<IndexedEdge>;
                                if (list == null)
                                {
                                    Hash[hash] = list = new List<IndexedEdge>(1);
                                    list.Add((IndexedEdge)value);
                                }
                                list.Add(edge);
                            }
                        }
                    }
        }

        public IEnumerable<EdgeLocation> FindMatching(EdgeInfo candidateEdge)
        {
            object value;
            if (Hash.TryGetValue(EdgeLookup.ComputeHash(candidateEdge), out value))
            {
                var list = value as List<IndexedEdge>;
                if (list != null)
                {
                    foreach (var entry in list)
                        if (EdgeLookup.MatchingEdges(entry.Shape, candidateEdge))
                            yield return entry.Location;
                }
                else
                {
                    var entry = value as IndexedEdge;
                    if (EdgeLookup.MatchingEdges(entry.Shape, candidateEdge))
                        yield return entry.Location;
                }
            }
        }
    }
}
