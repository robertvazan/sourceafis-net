using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Model
{
    public sealed class GapRemover : ISkeletonFilter
    {
        [DpiAdjusted]
        [Parameter(Lower = 0, Upper = 20)]
        public int RuptureSize = 5;
        [DpiAdjusted]
        [Parameter(Lower = 0, Upper = 100)]
        public int GapSize = 22;
        [Parameter(Upper = Angle.B90)]
        public byte GapAngle = Angle.FromDegreesB(30);
        [DpiAdjusted]
        [Parameter(Lower = 3, Upper = 40)]
        public int AngleSampleOffset = 22;
        [DpiAdjusted]
        [Parameter(Upper = 100)]
        public int ToleratedOverlapLength = 2;
        [DpiAdjusted]
        [Parameter(Upper = 20)]
        public int MinEndingLength = 7;

        [Nested]
        public KnotRemover KnotRemover = new KnotRemover();
        [Nested]
        public SkeletonShadow SkeletonShadow = new SkeletonShadow();

        public DetailLogger.Hook Logger = DetailLogger.Null;

        struct Gap
        {
            public SkeletonBuilder.Minutia End1;
            public SkeletonBuilder.Minutia End2;
        }

        public void Filter(SkeletonBuilder skeleton)
        {
            PriorityQueueF<Gap> queue = new PriorityQueueF<Gap>();
            foreach (SkeletonBuilder.Minutia end1 in skeleton.Minutiae)
                if (end1.Ridges.Count == 1 && end1.Ridges[0].Points.Count >= MinEndingLength)
                    foreach (SkeletonBuilder.Minutia end2 in skeleton.Minutiae)
                        if (end2 != end1 && end2.Ridges.Count == 1 && end1.Ridges[0].End != end2
                            && end2.Ridges[0].Points.Count >= MinEndingLength && IsWithinLimits(end1, end2))
                        {
                            Gap gap;
                            gap.End1 = end1;
                            gap.End2 = end2;
                            queue.Enqueue(Calc.DistanceSq(end1.Position, end2.Position), gap);
                        }

            BinaryMap shadow = SkeletonShadow.Draw(skeleton);
            while (queue.Count > 0)
            {
                Gap gap = queue.Dequeue();
                if (gap.End1.Ridges.Count == 1 && gap.End2.Ridges.Count == 1)
                {
                    Point[] line = Calc.ConstructLine(gap.End1.Position, gap.End2.Position);
                    if (!IsOverlapping(line, shadow))
                        AddRidge(skeleton, shadow, gap, line);
                }
            }

            KnotRemover.Filter(skeleton);
            Logger.Log(skeleton);
        }

        bool IsWithinLimits(SkeletonBuilder.Minutia end1, SkeletonBuilder.Minutia end2)
        {
            int distanceSq = Calc.DistanceSq(end1.Position, end2.Position);
            if (distanceSq <= Calc.Sq(RuptureSize))
                return true;
            if (distanceSq > Calc.Sq(GapSize))
                return false;

            byte gapDirection = Angle.AtanB(end1.Position, end2.Position);
            byte direction1 = Angle.AtanB(end1.Position, GetAngleSample(end1));
            if (Angle.Distance(direction1, Angle.Opposite(gapDirection)) > GapAngle)
                return false;
            byte direction2 = Angle.AtanB(end2.Position, GetAngleSample(end2));
            if (Angle.Distance(direction2, gapDirection) > GapAngle)
                return false;
            return true;
        }

        Point GetAngleSample(SkeletonBuilder.Minutia minutia)
        {
            SkeletonBuilder.Ridge ridge = minutia.Ridges[0];
            if (AngleSampleOffset < ridge.Points.Count)
                return ridge.Points[AngleSampleOffset];
            else
                return ridge.End.Position;
        }

        bool IsOverlapping(Point[] line, BinaryMap shadow)
        {
            for (int i = ToleratedOverlapLength; i < line.Length - ToleratedOverlapLength; ++i)
                if (shadow.GetBit(line[i]))
                    return true;
            return false;
        }

        void AddRidge(SkeletonBuilder skeleton, BinaryMap shadow, Gap gap, Point[] line)
        {
            SkeletonBuilder.Ridge ridge = new SkeletonBuilder.Ridge();
            foreach (Point point in line)
                ridge.Points.Add(point);
            ridge.Start = gap.End1;
            ridge.End = gap.End2;
            foreach (Point point in line)
                shadow.SetBitOne(point);
        }
    }
}
