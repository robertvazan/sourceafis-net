using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Matching
{
    public sealed class PairSelector
    {
        struct ConsideredPair
        {
            public MinutiaPair Pair;
            public float Distance;
        }

        PriorityQueue<ConsideredPair> Queue;

        public PairSelector()
        {
            Queue = new PriorityQueue<ConsideredPair>(
                delegate(ConsideredPair left, ConsideredPair right) { return Calc.Compare(left.Distance, right.Distance); });
        }

        public void Clear()
        {
            Queue.Clear();
        }

        public void Enqueue(MinutiaPair pair, float distance)
        {
            ConsideredPair added;
            added.Pair = pair;
            added.Distance = distance;
            Queue.Enqueue(added);
        }

        public void SkipPaired(MinutiaPairing pairing)
        {
            while (Queue.Count > 0 && (pairing.IsProbePaired(Queue.Peek().Pair.Probe)
                || pairing.IsCandidatePaired(Queue.Peek().Pair.Candidate)))
            {
                Queue.Dequeue();
            }
        }

        public int Count { get { return Queue.Count; } }

        public MinutiaPair Dequeue()
        {
            return Queue.Dequeue().Pair;
        }
    }
}
