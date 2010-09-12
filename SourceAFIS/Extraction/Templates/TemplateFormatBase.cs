using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SourceAFIS.Extraction.Templates
{
    public abstract class TemplateFormatBase<T> : ITemplateFormat<T> where T : class
    {
        public abstract T Export(TemplateBuilder builder);
        public abstract TemplateBuilder Import(T template);
        public abstract void Serialize(Stream stream, T template);
        public abstract T Deserialize(Stream stream);

        public byte[] Serialize(T template)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(stream, template);
            return stream.ToArray();
        }

        public T Deserialize(byte[] serialized)
        {
            MemoryStream stream = new MemoryStream(serialized);
            return Deserialize(stream);
        }

        public byte[] SerializeBuilder(TemplateBuilder builder)
        {
            return Serialize(Export(builder));
        }

        public TemplateBuilder DeserializeBuilder(byte[] serialized)
        {
            return Import(Deserialize(serialized));
        }
    }
}
