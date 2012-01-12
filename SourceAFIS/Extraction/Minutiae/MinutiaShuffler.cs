using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaShuffler
    {
        public void Shuffle(TemplateBuilder template)
        {
            int seed = 0;
            foreach (var minutia in template.Minutiae)
                seed += minutia.Direction + minutia.Position.X + minutia.Position.Y + (int)minutia.Type;
            template.Minutiae = Calc.Shuffle(template.Minutiae, new Random(seed)).ToList();
        }
    }
}
