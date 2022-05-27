// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Features;

namespace SourceAFIS.Extractor.Skeletons
{
    static class SkeletonDotFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            var removed = new List<SkeletonMinutia>();
            foreach (var minutia in skeleton.Minutiae)
                if (minutia.Ridges.Count == 0)
                    removed.Add(minutia);
            foreach (var minutia in removed)
                skeleton.RemoveMinutia(minutia);
        }
    }
}
