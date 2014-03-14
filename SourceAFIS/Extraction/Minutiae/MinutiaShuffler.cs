using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Minutiae
{
    public static class MinutiaShuffler
    {
        public static void Shuffle(FingerprintTemplate template)
        {
            int seed = 0;
            foreach (var minutia in template.Minutiae)
                seed += minutia.Direction + minutia.Position.X + minutia.Position.Y + (int)minutia.Type;
            template.Minutiae = Calc.Shuffle(template.Minutiae, new Random(seed)).ToList();
        }
    }
}
