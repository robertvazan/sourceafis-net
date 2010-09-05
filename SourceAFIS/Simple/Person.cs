using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using SourceAFIS.Dummy;

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
    public class Person : ICloneable
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

        List<Fingerprint> FingerprintList = new List<Fingerprint>();

        /// <summary>
        /// List of <see cref="Fingerprint"/>s belonging to the <see cref="Person"/>.
        /// </summary>
        /// <remarks>
        /// This collection is initially empty. Add <see cref="Fingerprint"/> objects
        /// here. You can also assign the whole collection.
        /// </remarks>
        public List<Fingerprint> Fingerprints
        {
            get { return FingerprintList; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                FingerprintList = value;
            }
        }

        /// <summary>
        /// Creates empty <see cref="Person"/> object.
        /// </summary>
        public Person()
        {
        }

        /// <summary>
        /// Creates new <see cref="Person"/> object and initializes it with
        /// a list of <see cref="Fingerprint"/>s.
        /// </summary>
        /// <param name="fingerprints"><see cref="Fingerprint"/> objects to add to the new <see cref="Person"/>.</param>
        public Person(params Fingerprint[] fingerprints)
        {
            Fingerprints = fingerprints.ToList();
        }

        /// <summary>
        /// Create deep copy of the <see cref="Person"/>.
        /// </summary>
        /// <returns>Deep copy of the <see cref="Person"/>.</returns>
        /// <remarks>
        /// This method also clones all <see cref="Fingerprint"/> objects contained
        /// in this <see cref="Person"/>.
        /// </remarks>
        public Person Clone()
        {
            Person clone = new Person();
            clone.Id = Id;
            foreach (Fingerprint fp in Fingerprints)
                clone.Fingerprints.Add(fp.Clone());
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }

        internal void CheckForNulls()
        {
            foreach (Fingerprint fp in Fingerprints)
                if (fp == null)
                    throw new ApplicationException("Person contains null Fingerprint references.");
        }
    }
}
