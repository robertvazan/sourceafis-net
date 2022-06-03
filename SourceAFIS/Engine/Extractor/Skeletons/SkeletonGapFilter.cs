// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class SkeletonGapFilter
    {
        static void AddGapRidge(BooleanMatrix shadow, SkeletonGap gap, IntPoint[] line)
        {
            var ridge = new SkeletonRidge();
            foreach (var point in line)
                ridge.Points.Add(point);
            ridge.Start = gap.End1;
            ridge.End = gap.End2;
            foreach (var point in line)
                shadow[point] = true;
        }
        static bool IsRidgeOverlapping(IntPoint[] line, BooleanMatrix shadow)
        {
            for (int i = Parameters.ToleratedGapOverlap; i < line.Length - Parameters.ToleratedGapOverlap; ++i)
                if (shadow[line[i]])
                    return true;
            return false;
        }
        static IntPoint AngleSampleForGapRemoval(SkeletonMinutia minutia)
        {
            var ridge = minutia.Ridges[0];
            if (Parameters.GapAngleOffset < ridge.Points.Count)
                return ridge.Points[Parameters.GapAngleOffset];
            else
                return ridge.End.Position;
        }
        static bool IsWithinGapLimits(SkeletonMinutia end1, SkeletonMinutia end2)
        {
            int distanceSq = (end1.Position - end2.Position).LengthSq;
            if (distanceSq <= Integers.Sq(Parameters.MaxRuptureSize))
                return true;
            if (distanceSq > Integers.Sq(Parameters.MaxGapSize))
                return false;
            double gapDirection = DoubleAngle.Atan(end1.Position, end2.Position);
            double direction1 = DoubleAngle.Atan(end1.Position, AngleSampleForGapRemoval(end1));
            if (DoubleAngle.Distance(direction1, DoubleAngle.Opposite(gapDirection)) > Parameters.MaxGapAngle)
                return false;
            double direction2 = DoubleAngle.Atan(end2.Position, AngleSampleForGapRemoval(end2));
            if (DoubleAngle.Distance(direction2, gapDirection) > Parameters.MaxGapAngle)
                return false;
            return true;
        }
        public static void Apply(Skeleton skeleton)
        {
            var queue = new PriorityQueue<SkeletonGap>();
            foreach (var end1 in skeleton.Minutiae)
                if (end1.Ridges.Count == 1 && end1.Ridges[0].Points.Count >= Parameters.ShortestJoinedEnding)
                    foreach (var end2 in skeleton.Minutiae)
                        if (end2 != end1 && end2.Ridges.Count == 1 && end1.Ridges[0].End != end2
                            && end2.Ridges[0].Points.Count >= Parameters.ShortestJoinedEnding && IsWithinGapLimits(end1, end2))
                        {
                            var gap = new SkeletonGap();
                            gap.Distance = (end1.Position - end2.Position).LengthSq;
                            gap.End1 = end1;
                            gap.End2 = end2;
                            queue.Add(gap);
                        }
            var shadow = skeleton.Shadow();
            while (queue.Count > 0)
            {
                var gap = queue.Remove();
                if (gap.End1.Ridges.Count == 1 && gap.End2.Ridges.Count == 1)
                {
                    var line = gap.End1.Position.LineTo(gap.End2.Position);
                    if (!IsRidgeOverlapping(line, shadow))
                        AddGapRidge(shadow, gap, line);
                }
            }
            SkeletonKnotFilter.Apply(skeleton);
            // https://sourceafis.machinezoo.com/transparency/removed-gaps
            FingerprintTransparency.Current.LogSkeleton("removed-gaps", skeleton);
        }
    }
}
