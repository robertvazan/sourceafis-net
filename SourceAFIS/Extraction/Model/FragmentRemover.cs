using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Model
{
    public sealed class FragmentRemover : ISkeletonFilter
    {
        [DpiAdjusted]
        [Parameter(Lower = 3, Upper = 100)]
        public int MinFragmentLength = 26;

        [Nested]
        public DotRemover DotRemover = new DotRemover();

        public DetailLogger.Hook Logger = DetailLogger.Null;

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
            Logger.Log(skeleton);
        }
    }
}
