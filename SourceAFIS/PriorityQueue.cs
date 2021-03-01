// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
	class PriorityQueue<T> where T : new()
	{
		readonly Comparer<T> Comparer;
		T[] Heap;
		int Size;

		public int Count { get { return Size; } }

		public PriorityQueue(Comparer<T> comparer)
		{
			Comparer = comparer;
			Heap = new T[1];
		}
		public PriorityQueue() : this(Comparer<T>.Default) { }

		public void Clear()
		{
			for (int i = 0; i < Size; ++i)
				Heap[i] = null;
			Size = 0;
		}
		void Enlarge()
		{
			T[] larger = new T[2 * Heap.Length];
			Array.Copy(Heap, larger, Heap.Length);
			Heap = larger;
		}
		static int Left(int parent) { return 2 * parent + 1; }
		static int Right(int parent) { return 2 * parent + 2; }
		static int Parent(int child) { return (child - 1) >> 1; }
		void BubbleUp(int bottom)
		{
			for (int child = bottom; child > 0; child = Parent(child))
			{
				int parent = Parent(child);
				if (Comparer.Compare(Heap[parent], Heap[child]) < 0)
					break;
				T tmp = Heap[child];
				Heap[child] = Heap[parent];
				Heap[parent] = tmp;
			}
		}
		public void Add(T item)
		{
			if (Size >= Heap.Length)
				Enlarge();
			Heap[Size] = item;
			BubbleUp(Size);
			++Size;
		}
		void BubbleDown()
		{
			int parent = 0;
			while (true)
			{
				int left = Left(parent);
				int right = Right(parent);
				if (left >= Size)
					break;
				int child;
				if (right >= Size || Comparer.Compare(Heap[left], Heap[right]) < 0)
					child = left;
				else
					child = right;
				if (Comparer.Compare(Heap[parent], Heap[child]) < 0)
					break;
				T tmp = Heap[parent];
				Heap[parent] = Heap[child];
				Heap[child] = tmp;
				parent = child;
			}
		}
		public T Peek()
		{
			if (Size <= 0)
				throw new InvalidOperationException();
			return Heap[0];
		}
		public T Remove()
		{
			if (Size <= 0)
				throw new InvalidOperationException();
			T result = Heap[0];
			Heap[0] = Heap[Size - 1];
			--Size;
			BubbleDown();
			return result;
		}
	}
}
