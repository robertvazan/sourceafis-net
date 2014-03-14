using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Collection of fingerprint-related information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains basic information (<see cref="Image"/>, <see cref="Template"/>) about the fingerprint that
    /// is used by SourceAFIS to perform template extraction and fingerprint matching.
    /// If you need to attach application-specific information to <see cref="Fingerprint"/> object,
    /// inherit from this class and add fields as necessary. <see cref="Fingerprint"/> objects can be
    /// grouped in <see cref="Person"/> objects.
    /// </para>
    /// <para>
    /// This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
    /// in application database, binary or XML files, or sent over network. You can either serialize
    /// the whole object or serialize individual properties. You can set some properties to <see langword="null"/>
    /// to exclude them from serialization.
    /// </para>
    /// </remarks>
    /// <seealso cref="Person"/>
    [Serializable]
    public class Fingerprint
    {
        /// <summary>
        /// Creates empty <see cref="Fingerprint"/> object.
        /// </summary>
        public Fingerprint() { }

        byte[,] ImageData;

        /// <summary>
        /// Fingerprint image.
        /// </summary>
        /// <value>
        /// Raw fingerprint image that was used to extract the <see cref="Template"/> or other image
        /// attached later after extraction. This property is <see langword="null"/> by default.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is the fingerprint image. This property must be set before call to <see cref="AfisEngine.Extract"/>
        /// in order to generate valid <see cref="Template"/>. Once the <see cref="Template"/> is generated, <see cref="Image"/> property has only
        /// informational meaning and it can be set to <see langword="null"/> to save space. It is however recommended to
        /// keep the original image just in case it is needed to regenerate the <see cref="Template"/> in future.
        /// </para>
        /// <para>
        /// The format of this image is a simple raw 2D array of <see langword="byte"/>s. Every byte
        /// represents shade of gray from black (0) to white (255). When indexing the 2D array, Y axis
        /// goes first, X axis goes second, e.g. <c>Image[y, x]</c>. To convert to/from <see cref="Bitmap"/>
        /// object, use <see cref="AsBitmap"/> property. To convert to/from <see cref="BitmapSource"/>
        /// object, use <see cref="AsBitmapSource"/> property.
        /// </para>
        /// <para>
        /// Accessors of this property do not clone the image. To avoid unwanted sharing of the <see langword="byte"/>
        /// array, call <see cref="ICloneable.Clone"/> on the <see cref="Image"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="Template"/>
        /// <seealso cref="AsBitmap"/>
        /// <seealso cref="AsBitmapSource"/>
        /// <seealso cref="AsImageData"/>
        /// <seealso cref="AfisEngine.Extract"/>
        [XmlIgnore]
        public byte[,] Image
        {
            get { return ImageData; }
            set
            {
                if (value == null)
                    ImageData = null;
                else
                {
                    if (value.GetLength(0) < 100 || value.GetLength(1) < 100)
                        throw new ApplicationException("Fingerprint image is too small.");
                    ImageData = value;
                }
            }
        }

        /// <summary>
        /// Fingerprint image as raw image in byte array.
        /// </summary>
        /// <value>
        /// Fingerprint image from <see cref="Image"/> property converted to raw image
        /// (one-dimensional byte array) or <see langword="null"/> if <see cref="Image"/>
        /// is <see langword="null"/>.
        /// </value>
        /// <seealso cref="Image"/>
        /// <seealso cref="AsBitmap"/>
        /// <seealso cref="AsBitmapSource"/>
        /// <seealso cref="Template"/>
        /// <seealso cref="AfisEngine.Extract"/>
        public byte[] AsImageData
        {
            get
            {
                byte[,] image = Image;
                if (image == null)
                    return null;
                else
                {
                    int height = image.GetLength(0);
                    int width = image.GetLength(1);

                    byte[] packed = new byte[8 + image.Length];
                    BitConverter.GetBytes(height).CopyTo(packed, 0);
                    BitConverter.GetBytes(width).CopyTo(packed, 4);

                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                            packed[8 + y * width + x] = image[y, x];

                    return packed;
                }
            }
            set
            {
                if (value == null)
                    Image = null;
                else
                {
                    if (value.Length <= 8)
                        throw new ApplicationException("Raw image array is too short.");
                    
                    int height = BitConverter.ToInt32(value, 0);
                    int width = BitConverter.ToInt32(value, 4);

                    if (height <= 0 || width <= 0)
                        throw new ApplicationException("Invalid image dimensions in raw image array.");
                    if (8 + width * height != value.Length)
                        throw new ApplicationException("Incorrect length of raw image array.");

                    byte[,] unpacked = new byte[height, width];
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                            unpacked[y, x] = value[8 + y * width + x];

                    Image = unpacked;
                }
            }
        }

        /// <summary>
        /// Fingerprint template.
        /// </summary>
        /// <value>
        /// Fingerprint template generated by <see cref="AfisEngine.Extract"/> or other template assigned
        /// for example after deserialization. This property is <see langword="null"/> by default.
        /// </value>
        /// <remarks>
        /// <para>
        /// Fingerprint template is an abstract model of the fingerprint that is serialized
        /// in a very compact binary format (up to a few KB). Templates are better than fingerprint images,
        /// because they require less space and they are easier to match than images. To generate
        /// <see cref="Template"/>, pass <see cref="Fingerprint"/> object with valid <see cref="Image"/> to <see cref="AfisEngine.Extract"/>.
        /// <see cref="Template"/> is required by <see cref="AfisEngine.Verify"/> and <see cref="AfisEngine.Identify"/>.
        /// </para>
        /// <para>
        /// Format of the template may change in later versions of SourceAFIS.
        /// Applications are recommended to keep the original <see cref="Image"/> in order to be able
        /// to regenerate the <see cref="Template"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="Image"/>
        /// <seealso cref="AfisEngine.Extract"/>
        public XElement Template
        {
            get { return Decoded != null ? Decoded.ToXml() : null; }
            set { Decoded = value != null ? new FingerprintTemplate(value) : null; }
        }

        Finger FingerPosition;

        /// <summary>
        /// Position of the finger on hand.
        /// </summary>
        /// <value>
        /// Finger (thumb to little) and hand (right or left) that was used to create this fingerprint.
        /// Default value <see cref="F:SourceAFIS.Simple.Finger.Any"/> means unspecified finger position.
        /// </value>
        /// <remarks>
        /// Finger position is used to speed up matching by skipping fingerprint pairs
        /// with incompatible finger positions. Check <see cref="SourceAFIS.Simple.Finger"/> enumeration for information
        /// on how to control this process. Default value <see cref="F:SourceAFIS.Simple.Finger.Any"/> disables this behavior.
        /// </remarks>
        /// <seealso cref="SourceAFIS.Simple.Finger"/>
        [XmlAttribute]
        public Finger Finger
        {
            get { return FingerPosition; }
            set
            {
                if (!Enum.IsDefined(typeof(Finger), value))
                    throw new ApplicationException("Invalid finger position.");
                FingerPosition = value;
            }
        }

        internal FingerprintTemplate Decoded;
    }
}
