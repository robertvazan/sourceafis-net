using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using SourceAFIS.General;

namespace SourceAFIS.Templates
{
    public sealed class CompactFormat : TemplateFormatBase<byte[]>
    {
        // Template format (all numbers are big-endian):
        // 4B magic
        // 1B version (current = 2)
        // 2B total length (including magic)
        // 2B original DPI (since version 2)
        // 2B original width (since version 2)
        // 2B original height (since version 2)
        // 2B minutia count
        // N*6B minutia records
        //      2B position X
        //      2B position Y
        //      1B direction
        //      1B type

        static readonly byte[] Magic = new byte[] { 0x50, 0xBC, 0xAF, 0x15 }; // read "SorcAFIS"

        public override byte[] Export(TemplateBuilder builder)
        {
            checked
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

                // 4B magic
                writer.Write(Magic);

                // 1B version (current = 2)
                writer.Write((byte)2);

                // 2B total length (including magic), will be filled later
                writer.Write((short)0);

                // 2B original DPI (since version 2)
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.OriginalDpi));

                // 2B original width (since version 2)
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.OriginalWidth));

                // 2B original height (since version 2)
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.OriginalHeight));

                // 2B minutia count
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.Minutiae.Count));

                // N*6B minutia records
                foreach (var minutia in builder.Minutiae)
                {
                    //      2B position X
                    writer.Write(IPAddress.HostToNetworkOrder((short)minutia.Position.X));

                    //      2B position Y
                    writer.Write(IPAddress.HostToNetworkOrder((short)minutia.Position.Y));

                    //      1B direction
                    writer.Write(minutia.Direction);

                    //      1B type
                    writer.Write((byte)minutia.Type);
                }

                writer.Close();

                // update length
                byte[] template = stream.ToArray();
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)template.Length)).CopyTo(template, 5);

                return template;
            }
        }

        public override TemplateBuilder Import(byte[] template)
        {
            TemplateBuilder builder = new TemplateBuilder();

            MemoryStream stream = new MemoryStream(template);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            // 4B magic
            for (int i = 0; i < Magic.Length; ++i)
                AssertException.Check(reader.ReadByte() == Magic[i]);

            // 1B version (current = 2)
            byte version = reader.ReadByte();
            AssertException.Check(version >= 1 && version <= 2);

            // 2B total length (including magic)
            reader.ReadInt16();

            if (version >= 2)
            {
                // 2B original DPI (since version 2)
                builder.OriginalDpi = IPAddress.NetworkToHostOrder(reader.ReadInt16());

                // 2B original width (since version 2)
                builder.OriginalWidth = IPAddress.NetworkToHostOrder(reader.ReadInt16());

                // 2B original height (since version 2)
                builder.OriginalHeight = IPAddress.NetworkToHostOrder(reader.ReadInt16());
            }

            // 2B minutia count
            int minutiaCount = IPAddress.NetworkToHostOrder(reader.ReadInt16());

            // N*6B minutia records
            for (int i = 0; i < minutiaCount; ++i)
            {
                TemplateBuilder.Minutia minutia = new TemplateBuilder.Minutia();

                //      2B position X
                minutia.Position.X = IPAddress.NetworkToHostOrder(reader.ReadInt16());

                //      2B position Y
                minutia.Position.Y = IPAddress.NetworkToHostOrder(reader.ReadInt16());

                //      1B direction
                minutia.Direction = reader.ReadByte();

                //      1B type
                minutia.Type = (TemplateBuilder.MinutiaType)reader.ReadByte();
                AssertException.Check(Enum.IsDefined(typeof(TemplateBuilder.MinutiaType), minutia.Type));

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
            stream.Read(header, 0, 7);

            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(header, 5));

            byte[] template = new byte[length];
            header.CopyTo(template, 0);
            stream.Read(template, 7, length - 7);
            return template;
        }
    }
}
