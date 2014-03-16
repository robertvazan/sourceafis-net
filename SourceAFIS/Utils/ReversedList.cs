using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Utils
{
    class ReversedList<T> : IList<T>
    {
        IList<T> Inner;

        public ReversedList(IList<T> inner)
        {
            Inner = inner;
        }

        public int IndexOf(T item)
        {
            for (int i = Count - 1; i >= 0; --i)
                if (Inner[i].Equals(item))
                    return Count - i - 1;
            return -1;
        }

        public void Insert(int position, T item)
        {
            Inner.Insert(Count - position, item);
        }

        public void RemoveAt(int position)
        {
            Inner.RemoveAt(Count - position - 1);
        }

        public T this[int index]
        {
            get { return Inner[Count - index - 1]; }
            set { Inner[Count - index - 1] = value; }
        }

        public void Add(T item)
        {
            Inner.Insert(0, item);
        }

        public void Clear()
        {
            Inner.Clear();
        }

        public bool Contains(T item)
        {
            return Inner.Contains(item);
        }

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

        public int Count { get { return Inner.Count; } }

        public bool IsReadOnly { get { return Inner.IsReadOnly; } }

        class Enumerator : IEnumerator<T>
        {
            IList<T> Inner;
            int Position;

            public Enumerator(IList<T> inner)
            {
                Inner = inner;
                Position = Inner.Count;
            }

            public T Current { get { return Inner[Position]; } }
            object IEnumerator.Current { get { return Inner[Position]; } }
            public void Dispose() { }
            public bool MoveNext()
            {
                if (Position >= 0)
                {
                    --Position;
                    return Position >= 0;
                }
                else
                    return false;
            }
            public void Reset() { Position = Inner.Count; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(Inner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(Inner);
        }
    }
}
