using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SourceAFIS.General;

namespace SourceAFIS.Templates
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
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, template);
        }

        public override Template Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as Template;
        }
    }
}
