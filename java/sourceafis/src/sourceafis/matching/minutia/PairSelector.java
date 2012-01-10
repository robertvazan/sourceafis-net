package sourceafis.matching.minutia;

import sourceafis.general.PriorityQueueF;

public class PairSelector
{
    PriorityQueueF<MinutiaPair> Queue = new PriorityQueueF<MinutiaPair>();

    public void Clear()
    {
        Queue.clear();
    }

    public void Enqueue(MinutiaPair pair, float distance)
    {
        Queue.enqueue(distance, pair);
    }

    public void SkipPaired(MinutiaPairing pairing)
    {
        while (Queue.size() > 0 && (pairing.IsProbePaired(Queue.peek().Probe)
            || pairing.IsCandidatePaired(Queue.peek().Candidate)))
        {
            MinutiaPair pair = (MinutiaPair)Queue.dequeue();
            if (pairing.GetCandidateByProbe(pair.Probe) == pair.Candidate)
                pairing.AddSupportByProbe(pair.Probe);
        }
    }

    public int getCount() {  return Queue.size();  }

    public MinutiaPair Dequeue()
    {
        return (MinutiaPair)Queue.dequeue();
    }
}

