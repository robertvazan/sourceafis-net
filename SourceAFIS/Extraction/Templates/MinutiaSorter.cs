using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Templates
{
    public sealed class MinutiaSorter
    {
        public void Sort(TemplateBuilder template)
        {
            int seed = 0;
            foreach (var minutia in template.Minutiae)
                seed += minutia.Direction + minutia.Position.X + minutia.Position.Y + (int)minutia.Type;
            template.Minutiae = Calc.Shuffle(template.Minutiae, new Random(seed)).ToList();
        }
    }
}
