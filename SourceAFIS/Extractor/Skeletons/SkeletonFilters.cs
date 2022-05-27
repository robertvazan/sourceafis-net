// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Features;

namespace SourceAFIS.Extractor.Skeletons
{
    static class SkeletonFilters
    {
        public static void Apply(Skeleton skeleton)
        {
            SkeletonDotFilter.Apply(skeleton);
            // https://sourceafis.machinezoo.com/transparency/removed-dots
            FingerprintTransparency.Current.LogSkeleton("removed-dots", skeleton);
            SkeletonPoreFilter.Apply(skeleton);
            SkeletonGapFilter.Apply(skeleton);
            SkeletonTailFilter.Apply(skeleton);
            SkeletonFragmentFilter.Apply(skeleton);
        }
    }
}
