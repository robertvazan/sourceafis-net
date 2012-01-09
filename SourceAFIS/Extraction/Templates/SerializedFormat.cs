using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
#if !COMPACT_FRAMEWORK
using System.Runtime.Serialization.Formatters.Binary;
#endif
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Templates
{
    public sealed class SerializedFormat : TemplateFormatBase<Template>
    {
        public override Template Export(TemplateBuilder builder)
        {
            return new Template(builder);
        }

        public override TemplateBuilder Import(Template template)
        {
            return template.ToTemplateBuilder();
        }

        public override void Serialize(Stream stream, Template template)
        {
#if !COMPACT_FRAMEWORK
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, template);
#else
            throw new AssertException();
#endif
        }

        public override Template Deserialize(Stream stream)
        {
#if !COMPACT_FRAMEWORK
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as Template;
#else
            throw new AssertException();
#endif
        }
    }
}
