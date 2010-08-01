using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Collection of <see cref="Fingerprint"/>s belonging to one person.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is primarily a way to group multiple <see cref="Fingerprint"/>s belonging to one person.
    /// This is very convenient feature when there are multiple fingerprints per person, because
    /// it is possible to match two <see cref="Person"/>s directly instead of iterating over their <see cref="Fingerprint"/>s.
    /// </para>
    /// <para>
    /// <see cref="Id"/> field is provided as simple means to bind <see cref="Person"/> objects to application-specific
    /// information. If you need more flexibility, inherit from <see cref="Person"/> class and add
    /// application-specific fields as necessary.
    /// </para>
    /// <para>
    /// This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
    /// in application database, binary or XML files, or sent over network. You can either serialize
    /// the whole <see cref="Person"/> or serialize individual <see cref="Fingerprint"/>s.
    /// </para>
    /// </remarks>
    /// <seealso cref="Fingerprint"/>
    [Serializable]
    public class Person : IList<Fingerprint>, ICloneable
    {
        /// <summary>
        /// Application-assigned ID for the <see cref="Person"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// SourceAFIS doesn't use this property. It is provided for applications as an easy means
        /// to link <see cref="Person"/> objects back to application-specific data. Applications can store any
        /// integer ID in this field, for example database table key or an array index.
        /// </para>
        /// <para>
        /// Applications that need to attach more detailed information to the person should
        /// inherit from <see cref="Person"/> class and add fields as necessary.
        /// </para>
        /// </remarks>
        [XmlAttribute]
        public int Id { get; set; }

        List<Fingerprint> InnerList = new List<Fingerprint>();

        /// <summary>
        /// Get/set all <see cref="Fingerprint"/>s belonging to this person as a collection.
        /// </summary>
        /// <value>
        /// An array containing all <see cref="Fingerprint"/>s belonging to this <see cref="Person"/>.
        /// </value>
        /// <seealso cref="this"/>
        public Fingerprint[] AllFingerprints
        {
            get { return InnerList.ToArray(); }
            set { InnerList = new List<Fingerprint>(value); }
        }

        /// <summary>
        /// Creates empty <see cref="Person"/> object.
        /// </summary>
        public Person() { }

        /// <summary>
        /// Get the number of <see cref="Fingerprint"/>s belonging to the <see cref="Person"/>.
        /// </summary>
        /// <value>
        /// Number of <see cref="Fingerprint"/>s belonging to the <see cref="Person"/>.
        /// </value>
        public int Count { get { return InnerList.Count; } }
        /// <summary>
        /// Add <see cref="Fingerprint"/> to person's fingerprint collection.
        /// </summary>
        /// <param name="fp">Fingerprint to add.</param>
        public void Add(Fingerprint fp) { CheckNull(fp); InnerList.Add(fp); }
        /// <summary>
        /// Remove all <see cref="Fingerprint"/>s from person's fingerprint collection.
        /// </summary>
        public void Clear() { InnerList.Clear(); }
        /// <summary>
        /// Remove <see cref="Fingerprint"/> from person's fingerprint collection.
        /// </summary>
        /// <param name="fp">Fingerprint to remove.</param>
        /// <returns>Returns <see langword="true"/> if the <see cref="Fingerprint"/> was found and removed, <see langword="false"/> otherwise.</returns>
        public bool Remove(Fingerprint fp) { return InnerList.Remove(fp); }
        /// <summary>
        /// Access <see cref="Person"/>'s fingerprint by index.
        /// </summary>
        /// <param name="index">Position of the <see cref="Fingerprint"/> within person's fingerprint collection.</param>
        /// <value>Fingerprint at the specified index.</value>
        /// <seealso cref="AllFingerprints"/>
        public Fingerprint this[int index]
        {
            get { return InnerList[index]; }
            set { CheckNull(value); InnerList[index] = value; }
        }
        /// <summary>
        /// Add <see cref="Fingerprint"/> to person's fingerprint collection at specific index.
        /// </summary>
        /// <param name="index">Index at which the fingerprint should be inserted.</param>
        /// <param name="fp">Fingerprint to insert.</param>
        public void Insert(int index, Fingerprint fp) { CheckNull(fp); InnerList.Insert(index, fp); }
        /// <summary>
        /// Remove <see cref="Fingerprint"/> from person's fingerprint collection at specified index.
        /// </summary>
        /// <param name="index">Index of the removed <see cref="Fingerprint"/> in person's fingerprint collection.</param>
        public void RemoveAt(int index) { InnerList.RemoveAt(index); }
        /// <summary>
        /// Create deep copy of the <see cref="Person"/>.
        /// </summary>
        /// <returns>Deep copy of the <see cref="Person"/>.</returns>
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
