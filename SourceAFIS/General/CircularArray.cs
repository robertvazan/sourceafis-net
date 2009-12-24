using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public sealed class CircularArray<T> : IList<T>
    {
        T[] Inner;
        int First;
        int ItemCount;
        int HeadCount { get { return Math.Min(ItemCount, Inner.Length - First); } }
        int TailCount { get { return Math.Max(0, First + ItemCount - Inner.Length); } }

        public CircularArray()
        {
            Inner = new T[16];
        }

        void CheckIndex(int index)
        {
            if (index < 0 || index >= ItemCount)
                throw new ArgumentOutOfRangeException();
        }

        int GetRealIndex(int index)
        {
            return index < HeadCount ? First + index : index - HeadCount;
        }

        void IncFirst()
        {
            ++First;
            if (First >= Inner.Length)
                First -= Inner.Length;
        }

        void DecFirst()
        {
            --First;
            if (First < 0)
                First += Inner.Length;
        }

        void Enlarge()
        {
            T[] enlarged = new T[2 * Inner.Length];
            for (int i = 0; i < ItemCount; ++i)
                enlarged[i] = Inner[GetRealIndex(i)];
            Inner = enlarged;
            First = 0;
        }

        void Move(int from, int to, int length)
        {
            if (from < to)
            {
                for (int i = length - 1; i >= 0; --i)
                    Inner[GetRealIndex(to + i)] = Inner[GetRealIndex(from + i)];
            }
            else
            {
                for (int i = 0; i < length; ++i)
                    Inner[GetRealIndex(to + i)] = Inner[GetRealIndex(from + i)];
            }
        }

        void MoveForward(int from, int length)
        {
            Move(from, from + 1, length);
        }

        void MoveBackward(int from, int length)
        {
            Move(from, from - 1, length);
        }

        void InsertSpaceForward(int index)
        {
            ++ItemCount;
            MoveForward(index, ItemCount - index - 1);
        }

        void InsertSpaceBackward(int index)
        {
            DecFirst();
            ++ItemCount;
            MoveBackward(1, index + 1);
        }

        void InsertSpace(int index)
        {
            if (ItemCount >= Inner.Length)
                Enlarge();
            if (index >= ItemCount / 2)
                InsertSpaceForward(index);
            else
                InsertSpaceBackward(index);
        }

        void RemoveSpaceForward(int index)
        {
            MoveBackward(index + 1, ItemCount - index - 1);
            --ItemCount;
        }

        void RemoveSpaceBackward(int index)
        {
            MoveForward(0, index);
            IncFirst();
            --ItemCount;
        }

        void RemoveSpace(int index)
        {
            if (index >= ItemCount / 2)
                RemoveSpaceForward(index);
            else
                RemoveSpaceBackward(index);
        }

        public int IndexOf(T item)
        {
            int index = Array.IndexOf<T>(Inner, item, First, Math.Min(ItemCount, Inner.Length - First));
            if (index >= 0)
                return index - First;
            else if (First + ItemCount > Inner.Length)
                return Array.IndexOf<T>(Inner, item, 0, First + ItemCount - Inner.Length);
            else
                return -1;
        }

        public void Insert(int index, T item)
        {
            CheckIndex(index);
            if (index > 0)
            {
                InsertSpace(index);
                Inner[GetRealIndex(index)] = item;
            }
            else
            {
                if (ItemCount >= Inner.Length)
                    Enlarge();
                DecFirst();
                ++ItemCount;
                Inner[GetRealIndex(0)] = item;
            }
        }

        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                IncFirst();
                --ItemCount;
            }
            else if (index == ItemCount - 1)
                --ItemCount;
            else
            {
                CheckIndex(index);
                RemoveSpace(index);
            }
        }

        public T this[int index]
        {
            get { CheckIndex(index); return Inner[GetRealIndex(index)]; }
            set { CheckIndex(index); Inner[GetRealIndex(index)] = value; }
        }

        public void Add(T item)
        {
            if (ItemCount >= Inner.Length)
                Enlarge();
            ++ItemCount;
            Inner[GetRealIndex(ItemCount - 1)] = item;
        }

        public void Clear()
        {
            First = 0;
            ItemCount = 0;
        }

        public bool Contains(T item)
        {
            return Array.IndexOf<T>(Inner, item, First, HeadCount) >= 0
                || Array.IndexOf<T>(Inner, item, 0, TailCount) >= 0;
        }

        public void CopyTo(T[] array, int at)
        {
            Array.Copy(Inner, First, array, at, HeadCount);
            Array.Copy(Inner, 0, array, at + HeadCount, TailCount);
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

        public int Count { get { return ItemCount; } }

        public bool IsReadOnly { get { return false; } }

        class Enumerator : IEnumerator<T>
        {
            CircularArray<T> Array;
            int Index;

            public Enumerator(CircularArray<T> array)
            {
                Array = array;
                Index = -1;
            }

            public T Current { get { return Array[Index]; } }
            object IEnumerator.Current { get { return Array[Index]; } }
            public void Dispose() { }
            public bool MoveNext()
            {
                if (Index < Array.ItemCount)
                {
                    ++Index;
                    return Index < Array.ItemCount;
                }
                else
                    return false;
            }
            public void Reset() { Index = -1; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
