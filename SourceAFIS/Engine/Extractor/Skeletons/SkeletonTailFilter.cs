// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class SkeletonTailFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 1 && minutia.Ridges[0].End.Ridges.Count >= 3)
                    if (minutia.Ridges[0].Points.Count < Parameters.MinTailLength)
                        minutia.Ridges[0].Detach();
            }
            SkeletonDotFilter.Apply(skeleton);
            SkeletonKnotFilter.Apply(skeleton);
            // https://sourceafis.machinezoo.com/transparency/removed-tails
            FingerprintTransparency.Current.LogSkeleton("removed-tails", skeleton);
        }
    }
}
