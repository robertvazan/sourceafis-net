using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Collection of fingerprint-related information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains basic information (image, template) about the fingerprint that
    /// is used by SourceAFIS to perform template extraction and fingerprint matching.
    /// If you need to attach application-specific information to Fingerprint object,
    /// inherit from this class and add fields as necessary.
    /// </para>
    /// <para>
    /// This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
    /// in application database, binary or XML files, or sent over network. You can either serialize
    /// the whole object or serialize individual properties. You can set some properties to null
    /// to exclude them from serialization.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Fingerprint : ICloneable
    {
        /// <summary>
        /// Creates empty Fingerprint object.
        /// </summary>
        public Fingerprint() { }

        Bitmap ImageValue;
        /// <summary>
        /// Fingerprint image.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Call <see cref="AfisEngine.Extract">AfisEngine.Extract(Fingerprint)</see> to convert this image
        /// to fingerprint template stored in <see cref="Template"/> property.
        /// </para>
        /// <para>
        /// If you later change <see cref="Image"/> property, <see cref="Template"/> property is not updated automatically unless
        /// you call <c>AfisEngine.Extract(Fingerprint)</c> again. You can even set this property to null
        /// in order to save space, because fingerprint matching requires only valid <see cref="Template"/> property.
        /// </para>
        /// <para>
        /// If you are going to alter the image after reading/writing this property,
        /// make a copy of the image by calling <see cref="M:Bitmap.Clone">Bitmap.Clone()</see> in order to avoid
        /// damaging the copy stored in this property.
        /// </para>
        /// </remarks>
        [XmlIgnore]
        public Bitmap Image { get { return ImageValue; } set { ImageValue = value; } }

        /// <summary>
        /// Fingerprint template.
        /// </summary>
        /// <remarks>
        /// Fingerprint template is an abstract model of the fingerprint that is serialized
        /// in a very compact binary format (up to a few KB). Templates are better than fingerprint images,
        /// because they require less space and they are easier to match than images.
        /// 
        /// Template property is initialized by <see cref="AfisEngine.Extract">AfisEngine.Extract(Fingerprint)</see>
        /// method that takes the fingerprint image from <see cref="Image"/> property and stores template
        /// in <c>Template</c> property. <c>Fingerprint</c> objects can be grouped in <see cref="Person"/> object
        /// and used in <see cref="AfisEngine.Verify">AfisEngine.Verify(Person,Person)</see> and
        /// <see cref="AfisEngine.Identify">AfisEngine.Identify(Person,IEnumerable&lt;Person&gt;)</see>.
        /// 
        /// If you need access to internal structure of the template, have a look at
        /// SourceAFIS.Extraction.Templates.SerializedFormat class in SourceAFIS source code.
        /// </remarks>
        [XmlAttribute]
        public byte[] Template
        {
            get { return Decoded != null ? new SerializedFormat().Serialize(Decoded) : null; }
            set { Decoded = value != null ? new SerializedFormat().Deserialize(value) : null; }
        }

        Finger FingerValue;
        /// <summary>
        /// Position of the finger on hand.
        /// </summary>
        [XmlAttribute]
        public Finger Finger { get { return FingerValue; } set { FingerValue = value; } }

        internal Template Decoded;

        /// <summary>
        /// Create deep copy of the Fingerprint.
        /// </summary>
        /// <returns></returns>
        public Fingerprint Clone()
        {
            Fingerprint clone = new Fingerprint();
            clone.Image = Image != null ? (Bitmap)Image.Clone() : null;
            clone.Template = Template != null ? (byte[])Template.Clone() : null;
            clone.Finger = Finger;
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
