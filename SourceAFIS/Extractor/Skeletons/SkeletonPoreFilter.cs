// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Configuration;
using SourceAFIS.Features;

namespace SourceAFIS.Extractor.Skeletons
{
    static class SkeletonPoreFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 3)
                {
                    for (int exit = 0; exit < 3; ++exit)
                    {
                        var exitRidge = minutia.Ridges[exit];
                        var arm1 = minutia.Ridges[(exit + 1) % 3];
                        var arm2 = minutia.Ridges[(exit + 2) % 3];
                        if (arm1.End == arm2.End && exitRidge.End != arm1.End && arm1.End != minutia && exitRidge.End != minutia)
                        {
                            var end = arm1.End;
                            if (end.Ridges.Count == 3 && arm1.Points.Count <= Parameters.MaxPoreArm && arm2.Points.Count <= Parameters.MaxPoreArm)
                            {
                                arm1.Detach();
                                arm2.Detach();
                                var merged = new SkeletonRidge();
                                merged.Start = minutia;
                                merged.End = end;
                                foreach (var point in minutia.Position.LineTo(end.Position))
                                    merged.Points.Add(point);
                            }
                            break;
                        }
                    }
                }
            }
            SkeletonKnotFilter.Apply(skeleton);
            // https://sourceafis.machinezoo.com/transparency/removed-pores
            FingerprintTransparency.Current.LogSkeleton("removed-pores", skeleton);
        }
    }
}
