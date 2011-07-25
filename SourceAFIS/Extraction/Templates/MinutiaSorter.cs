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
            template.Minutiae = Calc.Shuffle(template.Minutiae, new Random(0)).ToList();
        }
    }
}
