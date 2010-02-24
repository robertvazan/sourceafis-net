using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.General
{
    public sealed class PriorityQueue<T> where T : new()
    {
        List<T> Heap = new List<T>();
        IComparer<T> Comparer = Comparer<T>.Default;

        static int GetParent(int child) { return child / 2; }
        static int GetLeft(int parent) { return parent * 2; }
        static int GetRight(int parent) { return parent * 2 + 1; }
        static bool IsRoot(int node) { return node == 1; }
        bool IsValid(int node) { return node < Heap.Count; }
        int Compare(int left, int right) { return Comparer.Compare(Heap[left], Heap[right]); }

        public PriorityQueue()
        {
            Heap.Add(new T());
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            Comparer = comparer;
            Heap.Add(new T());
        }

        public PriorityQueue(Comparison<T> comparison)
        {
            Comparer = Calc.GetComparisonComparer<T>(comparison);
            Heap.Add(new T());
        }

        public void Clear()
        {
            Heap.Clear();
            Heap.Add(new T());
        }

        public int Count { get { return Heap.Count - 1; } }

        void BubbleUp(int bottom)
        {
            for (int bubble = bottom; !IsRoot(bubble); bubble = GetParent(bubble))
            {
                int parent = GetParent(bubble);
                if (Compare(bubble, parent) >= 0)
                    break;
                Calc.Swap(Heap, bubble, parent);
            }
        }

        public void Enqueue(T value)
        {
            Heap.Add(value);
            BubbleUp(Heap.Count - 1);
        }

        void BubbleDown()
        {
            int bubble = 1;
            while (true)
            {
                int left = GetLeft(bubble);
                int right = GetRight(bubble);
                if (!IsValid(left))
                    break;
                int child;
                if (!IsValid(right) || Compare(left, right) < 0)
                    child = left;
                else
                    child = right;
                if (Compare(bubble, child) < 0)
                    break;
                Calc.Swap(Heap, bubble, child);
                bubble = child;
            }
        }

        public T Peek()
        {
            if (Heap.Count == 1)
                throw new InvalidOperationException();
            return Heap[1];
        }

        public T Dequeue()
        {
            if (Heap.Count == 1)
                throw new InvalidOperationException();
            T result = Heap[1];
            Heap[1] = Heap[Heap.Count - 1];
            Heap.RemoveAt(Heap.Count - 1);
            BubbleDown();
            return result;
        }

        IEnumerator<T> GetEnumerator()
        {
            while (Heap.Count > 1)
                yield return Dequeue();
        }
    }
}
