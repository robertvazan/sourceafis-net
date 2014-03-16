using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Utils
{
    class PriorityQueueF<V> where V : new()
    {
        struct Item
        {
            public double Key;
            public V Value;
        }

        Item[] Heap;
        int ItemCount;

        public PriorityQueueF()
        {
            Heap = new Item[1];
        }

        public void Clear()
        {
            ItemCount = 0;
        }

        public int Count { get { return ItemCount; } }

        void Enlarge()
        {
            Item[] larger = new Item[2 * Heap.Length];
            Array.Copy(Heap, larger, Heap.Length);
            Heap = larger;
        }

        void BubbleUp(int bottom)
        {
            for (int bubble = bottom; bubble > 1; bubble = bubble >> 1)
            {
                int parent = bubble >> 1;
                if (Heap[parent].Key < Heap[bubble].Key)
                    break;
                Item tmp = Heap[bubble];
                Heap[bubble] = Heap[parent];
                Heap[parent] = tmp;
            }
        }

        public void Enqueue(double key, V value)
        {
            if (ItemCount + 1 >= Heap.Length)
                Enlarge();
            ++ItemCount;
            Heap[ItemCount].Key = key;
            Heap[ItemCount].Value = value;
            BubbleUp(ItemCount);
        }

        void BubbleDown()
        {
            int bubble = 1;
            while (true)
            {
                int left = bubble << 1;
                int right = (bubble << 1) + 1;
                if (left > ItemCount)
                    break;
                int child;
                if (right > ItemCount || Heap[left].Key < Heap[right].Key)
                    child = left;
                else
                    child = right;
                if (Heap[bubble].Key < Heap[child].Key)
                    break;
                Item tmp = Heap[bubble];
                Heap[bubble] = Heap[child];
                Heap[child] = tmp;
                bubble = child;
            }
        }

        public V Peek()
        {
            if (ItemCount <= 0)
                throw new InvalidOperationException();
            return Heap[1].Value;
        }

        public V Dequeue()
        {
            if (ItemCount == 0)
                throw new InvalidOperationException();
            V result = Heap[1].Value;
            Heap[1] = Heap[ItemCount];
            --ItemCount;
            BubbleDown();
            return result;
        }

        IEnumerator<V> GetEnumerator()
        {
            while (ItemCount > 0)
                yield return Dequeue();
        }
    }
}
