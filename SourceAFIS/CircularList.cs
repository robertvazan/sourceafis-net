// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS
{
    class CircularList<T> : IList<T>
    {
        readonly CircularArray<T> Inner = new CircularArray<T>(16);

        public int Count { get { return Inner.Size; } }
        public bool IsReadOnly { get { return false; } }
        public T this[int index]
        {
            get { return Inner[index]; }
            set { Inner[index] = value; }
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
            Inner.Insert(index, 1);
            Inner[index] = item;
        }
        public void RemoveAt(int index) { Inner.Remove(index, 1); }
        public void Add(T item)
        {
            Inner.Insert(Inner.Size, 1);
            Inner[Inner.Size - 1] = item;
        }
        public void Clear() { Inner.Remove(0, Inner.Size); }
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
                array[at + i] = Inner[i];
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
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<T>)this).GetEnumerator(); }
    }
}
