// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS.Engine.Primitives
{
    class ReversedList<T> : IList<T>
    {
        IList<T> inner;

        public int Count => inner.Count;
        public bool IsReadOnly => inner.IsReadOnly;
        public T this[int index]
        {
            get => inner[Count - index - 1];
            set => inner[Count - index - 1] = value;
        }

        public ReversedList(IList<T> inner) => this.inner = inner;

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; ++i)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return i;
            return -1;
        }
        public void Insert(int position, T item) => inner.Insert(Count - position, item);
        public void RemoveAt(int position) => inner.RemoveAt(Count - position - 1);
        public void Add(T item) => inner.Insert(0, item);
        public void Clear() => inner.Clear();
        public bool Contains(T item) => inner.Contains(item);
        public void CopyTo(T[] array, int at)
        {
            inner.CopyTo(array, at);
            Array.Reverse(array, at, Count);
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                inner.RemoveAt(Count - index - 1);
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
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
