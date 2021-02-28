// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS
{
	class ReversedList<T> : IList<T>
	{
		IList<T> Inner;

		public int Count { get { return Inner.Count; } }
		public bool IsReadOnly { get { return Inner.IsReadOnly; } }
		public T this[int index]
		{
			get { return Inner[Count - index - 1]; }
			set { Inner[Count - index - 1] = value; }
		}

		public ReversedList(IList<T> inner) { Inner = inner; }

		public int IndexOf(T item)
		{
			int index = Inner.LastIndexOf(item);
			return index >= 0 ? Count - index - 1 : -1;
		}
		public void Insert(int position, T item) { Inner.Insert(Count - position, item); }
		public void RemoveAt(int position) { Inner.RemoveAt(Count - position - 1); }
		public void Add(T item) { Inner.Insert(0, item); }
		public void Clear() { Inner.Clear(); }
		public bool Contains(T item) { return Inner.Contains(item); }
		public void CopyTo(T[] array, int at)
		{
			Inner.CopyTo(array, at);
			Array.Reverse(array, at, Count);
		}
		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index >= 0)
			{
				Inner.RemoveAt(Count - index - 1);
				return true;
			}
			else
				return false;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}
		IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<T>)this).GetEnumerator(); }
	}
}
