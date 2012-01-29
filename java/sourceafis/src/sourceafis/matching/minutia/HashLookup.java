package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.Hashtable;
 
 public final class HashLookup
    {
        EdgeLookup edgeLookup;
        Hashtable<Integer,Object> hash;
        EdgeShape currentCandidate;
        ArrayList<IndexedEdge> currentList;
        int currentOffset;

        public void reset(EdgeHash hash)
        {
            this.edgeLookup = hash.edgeLookup;
            this.hash = hash.hash;
            this.currentList = null;
        }

        @SuppressWarnings("unchecked")
		public IndexedEdge select(EdgeShape candidate)
        {
            //Object value;
            int ihash= edgeLookup.ComputeHash(candidate);
            //if (Hash.TryGetValue(EdgeLookup.ComputeHash(candidate), out value))
            if (hash.containsKey(ihash))
            { 
            	Object value=hash.get(ihash);
                //CurrentList = Hash.get(ihash);//value as List<IndexedEdge>;
                if (value instanceof IndexedEdge)//it can be IndexedEdge or ArrayList
                {
                    IndexedEdge entry = (IndexedEdge)value; 
                    if (edgeLookup.MatchingEdges(entry.shape, candidate))
                        return entry;
                    else
                        return null;
                }
                else// value is ArrayList
                {
                	currentList = (ArrayList<IndexedEdge>)value;
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