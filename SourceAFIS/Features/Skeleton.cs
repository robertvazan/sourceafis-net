// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Primitives;

namespace SourceAFIS.Features
{
    partial class Skeleton
    {
        public readonly SkeletonType Type;
        public readonly IntPoint Size;
        public readonly List<SkeletonMinutia> Minutiae = new List<SkeletonMinutia>();

        public Skeleton(SkeletonType type, IntPoint size)
        {
            Type = type;
            Size = size;
        }

        public void AddMinutia(SkeletonMinutia minutia) => Minutiae.Add(minutia);
        public void RemoveMinutia(SkeletonMinutia minutia) => Minutiae.Remove(minutia);
        public BooleanMatrix Shadow()
        {
            var shadow = new BooleanMatrix(Size);
            foreach (var minutia in Minutiae)
            {
                shadow[minutia.Position] = true;
                foreach (var ridge in minutia.Ridges)
                    if (ridge.Start.Position.Y <= ridge.End.Position.Y)
                        foreach (var point in ridge.Points)
                            shadow[point] = true;
            }
            return shadow;
        }
    }
}
