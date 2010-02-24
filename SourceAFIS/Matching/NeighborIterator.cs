using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class NeighborIterator
    {
        [DpiAdjusted]
        public int MaxDistance = 200;

        Template Template;

        public void Reset(Template template)
        {
            Template = template;
        }

        public IEnumerable<int> GetNeighbors(int minutia)
        {
            Point center = Template.Minutiae[minutia].Position;
            for (int i = 0; i < Template.Minutiae.Length; ++i)
                if (i != minutia && Calc.DistanceSq(center, Template.Minutiae[i].Position) <= Calc.Sq(MaxDistance))
                    yield return i;
        }
    }
}
