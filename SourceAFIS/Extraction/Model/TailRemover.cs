using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class TailRemover : ISkeletonFilter
    {
        const int MinTailLength = 21;

        public DotRemover DotRemover = new DotRemover();
        public KnotRemover KnotRemover = new KnotRemover();

        public void Filter(SkeletonBuilder skeleton)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 1 && minutia.Ridges[0].End.Ridges.Count >= 3)
                    if (minutia.Ridges[0].Points.Count < MinTailLength)
                        minutia.Ridges[0].Detach();
            }
            DotRemover.Filter(skeleton);
            KnotRemover.Filter(skeleton);
        }
    }
}
