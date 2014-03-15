using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Matching
{
    public sealed class HashLookup
    {
        Dictionary<int, object> Hash;
        EdgeShape CurrentCandidate;
        List<IndexedEdge> CurrentList;
        int CurrentOffset;

        public HashLookup(EdgeHash hash)
        {
            Hash = hash.Hash;
        }

        public IndexedEdge Select(EdgeShape candidate)
        {
            object value;
            if (Hash.TryGetValue(EdgeLookup.ComputeHash(candidate), out value))
            {
                CurrentList = value as List<IndexedEdge>;
                if (CurrentList == null)
                {
                    var entry = value as IndexedEdge;
                    if (EdgeLookup.MatchingEdges(entry.Shape, candidate))
                        return entry;
                    else
                        return null;
                }
                else
                {
                    for (CurrentOffset = 0; CurrentOffset < CurrentList.Count; ++CurrentOffset)
                        if (EdgeLookup.MatchingEdges(CurrentList[CurrentOffset].Shape, candidate))
                        {
                            CurrentCandidate = candidate;
                            return CurrentList[CurrentOffset++];
                        }
                    return null;
                }
            }
            else
                return null;
        }

        public IndexedEdge Next()
        {
            if (CurrentList == null)
                return null;
            while (CurrentOffset < CurrentList.Count)
            {
                if (EdgeLookup.MatchingEdges(CurrentList[CurrentOffset].Shape, CurrentCandidate))
                    return CurrentList[CurrentOffset++];
                ++CurrentOffset;
            }
            return null;
        }
    }
}
