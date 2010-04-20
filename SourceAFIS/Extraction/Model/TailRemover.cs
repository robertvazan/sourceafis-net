using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Model
{
    public sealed class TailRemover : ISkeletonFilter
    {
        [DpiAdjusted]
        [Parameter(Lower = 3, Upper = 100)]
        public int MinTailLength = 25;

        [Nested]
        public DotRemover DotRemover = new DotRemover();
        [Nested]
        public KnotRemover KnotRemover = new KnotRemover();

        public DetailLogger.Hook Logger = DetailLogger.Null;

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
            Logger.Log(skeleton);
        }
    }
}
