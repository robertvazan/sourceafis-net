using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class FragmentRemover : ISkeletonFilter
    {
        const int MinFragmentLength = 22;

        public DotRemover DotRemover = new DotRemover();

        public void Filter(SkeletonBuilder skeleton)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                if (minutia.Ridges.Count == 1)
                {
                    SkeletonBuilder.Ridge ridge = minutia.Ridges[0];
                    if (ridge.End.Ridges.Count == 1 && ridge.Points.Count < MinFragmentLength)
                        ridge.Detach();
                }
            DotRemover.Filter(skeleton);
        }
    }
}
