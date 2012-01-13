using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class PairSelector
    {
        PriorityQueueF<EdgePair> Queue = new PriorityQueueF<EdgePair>();

        public void Clear()
        {
            Queue.Clear();
        }

        public void Enqueue(EdgePair pair, float distance)
        {
            Queue.Enqueue(distance, pair);
        }

        public void SkipPaired(MinutiaPairing pairing)
        {
            while (Queue.Count > 0 && (pairing.IsProbePaired(Queue.Peek().Neighbor.Probe)
                || pairing.IsCandidatePaired(Queue.Peek().Neighbor.Candidate)))
            {
                EdgePair edge = Queue.Dequeue();
                if (pairing.IsProbePaired(edge.Neighbor.Probe) && pairing.GetByProbe(edge.Neighbor.Probe).Pair.Candidate == edge.Neighbor.Candidate)
                    pairing.AddSupportByProbe(edge.Neighbor.Probe);
            }
        }

        public int Count { get { return Queue.Count; } }

        public EdgePair Dequeue()
        {
            return Queue.Dequeue();
        }
    }
}
