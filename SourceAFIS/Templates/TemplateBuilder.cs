using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Templates
{
    public sealed class TemplateBuilder : ICloneable
    {
        public enum MinutiaType
        {
            Ending = 0,
            Bifurcation = 1,
            Other = 2
        }

        public class Minutia
        {
            public Point Position;
            public byte Direction;
            public MinutiaType Type;
        }

        public int OriginalDpi;
        public int OriginalWidth;
        public int OriginalHeight;

        public int StandardDpiWidth
        {
            get { return Calc.DivRoundUp(OriginalWidth * 500, OriginalDpi); }
            set { OriginalWidth = value * OriginalDpi / 500; }
        }
        public int StandardDpiHeight
        {
            get { return Calc.DivRoundUp(OriginalHeight * 500, OriginalDpi); }
            set { OriginalHeight = value * OriginalDpi / 500; }
        }

        public List<Minutia> Minutiae = new List<Minutia>();

        public TemplateBuilder Clone()
        {
            return new Template(this).ToTemplateBuilder();
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
