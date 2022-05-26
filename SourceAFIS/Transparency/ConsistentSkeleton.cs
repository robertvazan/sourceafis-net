// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Features;
using SourceAFIS.Primitives;

namespace SourceAFIS.Transparency
{
    class ConsistentSkeleton
    {
        public int Width;
        public int Height;
        public List<IntPoint> Minutiae;
        public List<ConsistentSkeletonRidge> Ridges;
        public ConsistentSkeleton(Skeleton skeleton)
        {
            Width = skeleton.Size.X;
            Height = skeleton.Size.Y;
            var offsets = new Dictionary<SkeletonMinutia, int>();
            for (int i = 0; i < skeleton.Minutiae.Count; ++i)
                offsets[skeleton.Minutiae[i]] = i;
            Minutiae = (from m in skeleton.Minutiae select m.Position).ToList();
            Ridges = (
                from m in skeleton.Minutiae
                from r in m.Ridges
                where r.Points is CircularList<IntPoint>
                select new ConsistentSkeletonRidge
                {
                    Start = offsets[r.Start],
                    End = offsets[r.End],
                    Points = r.Points
                }).ToList();
        }
    }
}
