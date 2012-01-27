package sourceafis.matching.minutia;

import sourceafis.general.PriorityQueueF;

public final class PairSelector
{
    PriorityQueueF<EdgePair> queue = new PriorityQueueF<EdgePair>();

    public void clear()
    {
        queue.clear();
    }

    public void enqueue(EdgePair pair, float distance)
    {
        queue.enqueue(distance, pair);
    }

    public void skipPaired(MinutiaPairing pairing)
    {
        while (queue.size() > 0 && (pairing.isProbePaired(queue.peek().neighbor.probe)
            || pairing.isCandidatePaired(queue.peek().neighbor.candidate)))
        {
            EdgePair edge = (EdgePair)queue.dequeue();
            if (pairing.isProbePaired(edge.neighbor.probe) && pairing.getByProbe(edge.neighbor.probe).pair.candidate == edge.neighbor.candidate) {
            	pairing.addSupportByProbe(edge.reference.probe);
                pairing.addSupportByProbe(edge.neighbor.probe);
            }
        }
    }

    public int getCount() {  return queue.size();  }

    public EdgePair dequeue()
    {
        return  queue.dequeue();
    }
}

