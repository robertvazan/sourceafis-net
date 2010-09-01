using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;

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
