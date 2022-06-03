// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS.Engine.Primitives
{
    class CircularList<T> : IList<T>
    {
        readonly CircularArray<T> inner = new CircularArray<T>(16);

        public int Count => inner.Size;
        public bool IsReadOnly => false;
        public T this[int index]
        {
            get => inner[index];
            set => inner[index] = value;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; ++i)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return i;
            return -1;
        }
        public void Insert(int index, T item)
        {
            inner.Insert(index, 1);
            inner[index] = item;
        }
        public void RemoveAt(int index) => inner.Remove(index, 1);
        public void Add(T item)
        {
            inner.Insert(inner.Size, 1);
            inner[inner.Size - 1] = item;
        }
        public void Clear() { inner.Remove(0, inner.Size); }
        public bool Contains(T item)
        {
            for (int i = 0; i < Count; ++i)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return true;
            return false;
        }
        public void CopyTo(T[] array, int at)
        {
            for (int i = 0; i < Count; ++i)
                array[at + i] = inner[i];
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
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
