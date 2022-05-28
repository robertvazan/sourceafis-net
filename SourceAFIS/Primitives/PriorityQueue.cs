// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS.Primitives
{
    class PriorityQueue<T>
        where T : class
    {
        readonly Comparer<T> comparer;
        T[] heap;
        int size;

        public int Count => size;

        public PriorityQueue(Comparer<T> comparer)
        {
            this.comparer = comparer;
            heap = new T[1];
        }
        public PriorityQueue() : this(Comparer<T>.Default) { }

        public void Clear()
        {
            for (int i = 0; i < size; ++i)
                heap[i] = null;
            size = 0;
        }
        void Enlarge()
        {
            T[] larger = new T[2 * heap.Length];
            Array.Copy(heap, larger, heap.Length);
            heap = larger;
        }
        static int Left(int parent) => 2 * parent + 1;
        static int Right(int parent) => 2 * parent + 2;
        static int Parent(int child) => child - 1 >> 1;
        void BubbleUp(int bottom)
        {
            for (int child = bottom; child > 0; child = Parent(child))
            {
                int parent = Parent(child);
                if (comparer.Compare(heap[parent], heap[child]) < 0)
                    break;
                T tmp = heap[child];
                heap[child] = heap[parent];
                heap[parent] = tmp;
            }
        }
        public void Add(T item)
        {
            if (size >= heap.Length)
                Enlarge();
            heap[size] = item;
            BubbleUp(size);
            ++size;
        }
        void BubbleDown()
        {
            int parent = 0;
            while (true)
            {
                int left = Left(parent);
                int right = Right(parent);
                if (left >= size)
                    break;
                int child;
                if (right >= size || comparer.Compare(heap[left], heap[right]) < 0)
                    child = left;
                else
                    child = right;
                if (comparer.Compare(heap[parent], heap[child]) < 0)
                    break;
                T tmp = heap[parent];
                heap[parent] = heap[child];
                heap[child] = tmp;
                parent = child;
            }
        }
        public T Peek()
        {
            if (size <= 0)
                throw new InvalidOperationException();
            return heap[0];
        }
        public T Remove()
        {
            if (size <= 0)
                throw new InvalidOperationException();
            T result = heap[0];
            heap[0] = heap[size - 1];
            --size;
            BubbleDown();
            return result;
        }
    }
}
