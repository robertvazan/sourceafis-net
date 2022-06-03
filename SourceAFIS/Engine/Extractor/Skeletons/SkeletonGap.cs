// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Engine.Features;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    class SkeletonGap : IComparable<SkeletonGap>
    {
        public int Distance;
        public SkeletonMinutia End1;
        public SkeletonMinutia End2;
        public int CompareTo(SkeletonGap other)
        {
            int distanceCmp = Distance.CompareTo(other.Distance);
            if (distanceCmp != 0)
                return distanceCmp;
            int end1Cmp = End1.Position.CompareTo(other.End1.Position);
            if (end1Cmp != 0)
                return end1Cmp;
            return End2.Position.CompareTo(other.End2.Position);
        }
    }
}
