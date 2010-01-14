using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.Extraction.Templates
{
    public sealed class TemplateBuilder
    {
        public enum MinutiaType
        {
            Ending = 0,
            Bifurcation = 1
        }

        public class Minutia
        {
            public Point Position;
            public byte Direction;
            public MinutiaType Type;
        }

        public List<Minutia> Minutiae = new List<Minutia>();
    }
}
