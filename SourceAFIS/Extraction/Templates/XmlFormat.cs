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
                new XAttribute("Version", "1"),
                from minutia in builder.Minutiae
                select new XElement("Minutia",
                    new XAttribute("X", minutia.Position.X),
                    new XAttribute("Y", minutia.Position.Y),
                    new XAttribute("Direction", minutia.Direction),
                    new XAttribute("Type", minutia.Type.ToString())));
        }

        public override TemplateBuilder Import(XElement template)
        {
            if ((int)template.Attribute("Version") != 1)
                throw new ApplicationException("Unknown template version.");
            return new TemplateBuilder()
            {
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
