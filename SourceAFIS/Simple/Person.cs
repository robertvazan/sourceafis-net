using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Simple
{
    [Serializable]
    public class Person : IList<Fingerprint>, ICloneable
    {
        public int Id;

        List<Fingerprint> InnerList = new List<Fingerprint>();

        public int Count { get { return InnerList.Count; } }
        public void Add(Fingerprint fp) { CheckNull(fp); InnerList.Add(fp); }
        public void Clear() { InnerList.Clear(); }
        public bool Remove(Fingerprint fp) { return InnerList.Remove(fp); }
        public Fingerprint this[int index]
        {
            get { return InnerList[index]; }
            set { CheckNull(value); InnerList[index] = value; }
        }
        public void Insert(int index, Fingerprint fp) { CheckNull(fp); InnerList.Insert(index, fp); }
        public void RemoveAt(int index) { InnerList.RemoveAt(index); }
        public Person Clone()
        {
            Person clone = new Person();
            foreach (Fingerprint fp in InnerList)
                clone.Add(fp.Clone());
            return clone;
        }

        IEnumerator<Fingerprint> IEnumerable<Fingerprint>.GetEnumerator() { return InnerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return InnerList.GetEnumerator(); }
        bool ICollection<Fingerprint>.IsReadOnly { get { return false; } }
        bool ICollection<Fingerprint>.Contains(Fingerprint fp) { return InnerList.Contains(fp); }
        void ICollection<Fingerprint>.CopyTo(Fingerprint[] array, int index) { InnerList.CopyTo(array, index); }
        int IList<Fingerprint>.IndexOf(Fingerprint fp) { return InnerList.IndexOf(fp); }
        object ICloneable.Clone() { return Clone(); }

        void CheckNull(Fingerprint fp)
        {
            if (fp == null)
                throw new ArgumentNullException();
        }
    }
}
