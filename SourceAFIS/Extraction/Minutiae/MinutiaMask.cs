using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaMask
    {
        const float DirectedExtension = 10.06f;

        public void Filter(FingerprintTemplate template, BinaryMap mask)
        {
            template.Minutiae.RemoveAll(minutia =>
            {
                var arrow = Calc.Round(-DirectedExtension * Angle.ToVector(minutia.Direction));
                return !mask.GetBitSafe((Point)minutia.Position + new Size(arrow), false);
            });
        }
    }
}
