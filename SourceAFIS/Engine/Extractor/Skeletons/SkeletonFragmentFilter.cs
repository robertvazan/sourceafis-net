// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class SkeletonFragmentFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
                if (minutia.Ridges.Count == 1)
                {
                    var ridge = minutia.Ridges[0];
                    if (ridge.End.Ridges.Count == 1 && ridge.Points.Count < Parameters.MinFragmentLength)
                        ridge.Detach();
                }
            SkeletonDotFilter.Apply(skeleton);
            // https://sourceafis.machinezoo.com/transparency/removed-fragments
            FingerprintTransparency.Current.LogSkeleton("removed-fragments", skeleton);
        }
    }
}
