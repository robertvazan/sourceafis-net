using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Extraction.Templates
{
    public sealed class XmlFormat : TemplateFormatBase<XElement>
    {
        public override XElement Export(TemplateBuilder builder)
        {
            return new XElement("FingerprintTemplate",
                new XAttribute("Version", "2"),
                new XAttribute("Width", builder.Width),
                new XAttribute("Height", builder.Height),
                from minutia in builder.Minutiae
                select new XElement("Minutia",
                    new XAttribute("X", minutia.Position.X),
                    new XAttribute("Y", minutia.Position.Y),
                    new XAttribute("Direction", minutia.Direction),
                    new XAttribute("Type", minutia.Type.ToString())));
        }

        public override TemplateBuilder Import(XElement template)
        {
            int version = (int)template.Attribute("Version");
            if (version < 1 || version > 2)
                throw new ApplicationException("Unknown template version.");
            return new TemplateBuilder()
            {
                Width = version >= 2 ? (int)template.Attribute("Width") : 0,
                Height = version >= 2 ? (int)template.Attribute("Height") : 0,
                Minutiae = (from minutia in template.Elements("Minutia")
                            select new TemplateBuilder.Minutia()
                            {
                                Position = new Point(
                                    (int)minutia.Attribute("X"),
                                    (int)minutia.Attribute("Y")),
                                Direction = (byte)(uint)minutia.Attribute("Direction"),
                                Type = (TemplateBuilder.MinutiaType)Enum.Parse(
                                    typeof(TemplateBuilder.MinutiaType),
                                    (string)minutia.Attribute("Type"),
                                    false)
                            }).ToList()
            };
        }

        public override void Serialize(Stream stream, XElement template)
        {
            template.Save(stream);
        }

        public override XElement Deserialize(Stream stream)
        {
            return XElement.Load(stream);
        }
    }
}
