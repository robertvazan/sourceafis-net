using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SourceAFIS.Dummy;

namespace SourceAFIS.Extraction.Templates
{
    public sealed class CompactFormat : TemplateFormatBase<byte[]>
    {
        // Template format (all numbers are big-endian):
        // 4B magic
        // 1B version (current = 1)
        // 2B total length (including magic)
        // 2B minutia count
        // N*6B minutia records
        //      2B position X
        //      2B position Y
        //      1B direction
        //      1B type

        static readonly byte[] Magic = new byte[] { 0x50, 0xBC, 0xAF, 0x15 }; // read "SorcAFIS"

        byte[] FixByteOrder(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                return bytes.Reverse().ToArray();
            else
                return bytes;
        }

        void EncodeShort(short value, byte[] template, int offset)
        {
            template[offset] = (byte)(((ushort)value) >> 8);
            template[offset + 1] = (byte)(((ushort)value) & 0xFF);
        }

        void EncodeShort(int value, byte[] template, int offset)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new ArgumentOutOfRangeException();
            EncodeShort((short)value, template, offset);
        }

        public override byte[] Export(TemplateBuilder builder)
        {
            byte[] template = new byte[4 + 1 + 2 + 2 + 6 * builder.Minutiae.Count];

            // 4B magic
            Magic.CopyTo(template, 0);

            // 1B version
            template[4] = 1;

            // 2B total length, filled later
            EncodeShort(template.Length, template, 5);

            // 2B minutia count
            EncodeShort(builder.Minutiae.Count, template, 7);

            for (int i = 0; i < builder.Minutiae.Count; ++i)
            {
                // 2B position X
                EncodeShort(builder.Minutiae[i].Position.X, template, 9 + 6 * i);
                // 2B position Y
                EncodeShort(builder.Minutiae[i].Position.Y, template, 9 + 6 * i + 2);
                // 1B direction
                template[9 + 6 * i + 4] = builder.Minutiae[i].Direction;
                // 1B type
                template[9 + 6 * i + 5] = (byte)builder.Minutiae[i].Type;
            }

            return template;
        }

        void DecodeConstraint(bool condition)
        {
            if (!condition)
                throw new ApplicationException("Incorrect template format.");
        }

        short DecodeShort(byte[] template, int offset)
        {
            return (short)((template[offset] << 8) | template[offset + 1]);
        }

        public override TemplateBuilder Import(byte[] template)
        {
            TemplateBuilder builder = new TemplateBuilder();

            // 4B magic
            for (int i = 0; i < Magic.Length; ++i)
                DecodeConstraint(template[i] == Magic[i]);

            // 1B version (current = 1)
            DecodeConstraint(template[4] == 1);

            // 2B total length (including magic)
            DecodeConstraint(DecodeShort(template, 5) == template.Length);

            // 2B minutia count
            int minutiaCount = DecodeShort(template, 7);
            DecodeConstraint(9 + 6 * minutiaCount == template.Length);

            for (int i = 0; i < minutiaCount; ++i)
            {
                TemplateBuilder.Minutia minutia = new TemplateBuilder.Minutia();

                // 2B position X
                minutia.Position.X = DecodeShort(template, 9 + 6 * i);

                // 2B position Y
                minutia.Position.Y = DecodeShort(template, 9 + 6 * i + 2);

                // 1B direction
                minutia.Direction = template[9 + 6 * i + 4];

                // 1B type
                minutia.Type = (TemplateBuilder.MinutiaType)template[9 + 6 * i + 5];
                DecodeConstraint(Enum.IsDefined(typeof(TemplateBuilder.MinutiaType), minutia.Type));

                builder.Minutiae.Add(minutia);
            }

            return builder;
        }

        public override void Serialize(Stream stream, byte[] template)
        {
            stream.Write(template, 0, template.Length);
        }

        public override byte[] Deserialize(Stream stream)
        {
            byte[] header = new byte[7];
            int headerBytes = stream.Read(header, 0, 7);
            DecodeConstraint(headerBytes == 7);

            for (int i = 0; i < Magic.Length; ++i)
                DecodeConstraint(header[i] == Magic[i]);
            DecodeConstraint(header[4] == 1);

            int length = DecodeShort(header, 5);
            DecodeConstraint(length > header.Length);

            byte[] template = new byte[length];
            header.CopyTo(template, 0);
            int templateBytes = stream.Read(template, 7, length - 7);
            DecodeConstraint(7 + templateBytes == length);

            return template;
        }
    }
}
