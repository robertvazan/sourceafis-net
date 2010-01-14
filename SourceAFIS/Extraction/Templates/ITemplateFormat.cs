using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SourceAFIS.Extraction.Templates
{
    public interface ITemplateFormat<T> where T : class
    {
        T Export(TemplateBuilder builder);
        TemplateBuilder Import(T template);
        void Serialize(Stream stream, T template);
        T Deserialize(Stream stream);
        
        byte[] Serialize(T template);
        T Deserialize(byte[] serialized);
        byte[] SerializeBuilder(TemplateBuilder builder);
        TemplateBuilder DeserializeBuilder(byte[] serialized);
    }
}
