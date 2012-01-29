package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.HashMap;
 
 public final class HashLookup
    {
        EdgeLookup edgeLookup;
        HashMap<Integer,Object> hash;
        EdgeShape currentCandidate;
        ArrayList<IndexedEdge> currentList;
        int currentOffset;

        public void reset(EdgeHash hash)
        {
            this.edgeLookup = hash.edgeLookup;
            this.hash = hash.hash;
        }

        @SuppressWarnings("unchecked")
		public IndexedEdge select(EdgeShape candidate)
        {
        	Object value = hash.get(edgeLookup.ComputeHash(candidate));
            if (value != null)
            {
            	currentList = value instanceof ArrayList<?> ? (ArrayList<IndexedEdge>)value : null; 
                if (currentList == null)//it can be IndexedEdge or ArrayList
                {
                    IndexedEdge entry = (IndexedEdge)value; 
                    if (edgeLookup.MatchingEdges(entry.shape, candidate))
                        return entry;
                    else
                        return null;
                }
                else// value is ArrayList
                {
                    for (currentOffset = 0; currentOffset < currentList.size(); ++currentOffset)
                        if (edgeLookup.MatchingEdges(currentList.get(currentOffset).shape, candidate))
                        {
                            currentCandidate = candidate;
                            return currentList.get(currentOffset++);
                        }
                    return null;
                }
            }
            else
                return null;
        }

        public IndexedEdge next()
        {
            if (currentList == null)
                return null;
            while (currentOffset < currentList.size())
            {
                if (edgeLookup.MatchingEdges(currentList.get(currentOffset).shape, currentCandidate))
                    return currentList.get(currentOffset++);
                ++currentOffset;
            }
            return null;
        }
    }