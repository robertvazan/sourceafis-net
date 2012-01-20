using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaMask
    {
        [DpiAdjusted(Min = 0)]
        [Parameter(Lower = 0, Upper = 50)]
        public float DirectedExtension = 10.06f;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Filter(TemplateBuilder template, BinaryMap mask)
        {
            template.Minutiae.RemoveAll(minutia =>
            {
                var arrow = Calc.Round(-DirectedExtension * Angle.ToVector(minutia.Direction));
                return !mask.GetBitSafe((Point)minutia.Position + new Size(arrow), false);
            });
            Logger.Log(template);
        }
    }
}
