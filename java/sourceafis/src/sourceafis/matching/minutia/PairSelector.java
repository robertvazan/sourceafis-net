package sourceafis.matching.minutia;

import sourceafis.general.PriorityQueueF;

public class PairSelector
{
    PriorityQueueF<MinutiaPair> Queue = new PriorityQueueF<MinutiaPair>();

    public void Clear()
    {
        Queue.Clear();
    }

    public void Enqueue(MinutiaPair pair, float distance)
    {
        Queue.Enqueue(distance, pair);
    }

    public void SkipPaired(MinutiaPairing pairing)
    {
        while (Queue.getCount() > 0 && (pairing.IsProbePaired(Queue.Peek().Probe)
            || pairing.IsCandidatePaired(Queue.Peek().Candidate)))
        {
            MinutiaPair pair = (MinutiaPair)Queue.Dequeue();
            if (pairing.GetCandidateByProbe(pair.Probe) == pair.Candidate)
                pairing.AddSupportByProbe(pair.Probe);
        }
    }

    public int getCount() {  return Queue.getCount();  }

    public MinutiaPair Dequeue()
    {
        return (MinutiaPair)Queue.Dequeue();
    }
}

