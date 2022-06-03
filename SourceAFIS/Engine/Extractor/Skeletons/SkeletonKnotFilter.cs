// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class SkeletonKnotFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 2 && minutia.Ridges[0].Reversed != minutia.Ridges[1])
                {
                    var extended = minutia.Ridges[0].Reversed;
                    var removed = minutia.Ridges[1];
                    if (extended.Points.Count < removed.Points.Count)
                    {
                        var tmp = extended;
                        extended = removed;
                        removed = tmp;
                        extended = extended.Reversed;
                        removed = removed.Reversed;
                    }
                    extended.Points.RemoveAt(extended.Points.Count - 1);
                    foreach (var point in removed.Points)
                        extended.Points.Add(point);
                    extended.End = removed.End;
                    removed.Detach();
                }
            }
            SkeletonDotFilter.Apply(skeleton);
        }
    }
}
