using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Simple
{
    [Serializable]
    public class Fingerprint : ICloneable
    {
        public Bitmap Image;
        public byte[] Template
        {
            get { return Decoded != null ? new SerializedFormat().Serialize(Decoded) : null; }
            set { Decoded = value != null ? new SerializedFormat().Deserialize(value) : null; }
        }
        public Finger Finger;

        internal Template Decoded;

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
