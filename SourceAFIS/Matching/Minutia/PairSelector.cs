using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class PairSelector
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
            while (Queue.Count > 0 && (pairing.IsProbePaired(Queue.Peek().Probe)
                || pairing.IsCandidatePaired(Queue.Peek().Candidate)))
            {
                MinutiaPair pair = Queue.Dequeue();
                if (pairing.GetCandidateByProbe(pair.Probe) == pair.Candidate)
                    pairing.AddSupportByProbe(pair.Probe);
            }
        }

        public int Count { get { return Queue.Count; } }

        public MinutiaPair Dequeue()
        {
            return Queue.Dequeue();
        }
    }
}
