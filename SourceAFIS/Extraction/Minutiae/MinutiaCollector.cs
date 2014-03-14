using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.Extraction.Minutiae
{
    public sealed class MinutiaCollector
    {
        const int DirectionSegmentLength = 21;
        const int DirectionSegmentSkip = 1;

        byte ComputeDirection(SkeletonBuilder.Ridge ridge)
        {
            int first = DirectionSegmentSkip;
            int last = DirectionSegmentSkip + DirectionSegmentLength - 1;
            
            if (last >= ridge.Points.Count)
            {
                int shift = last - ridge.Points.Count + 1;
                last -= shift;
                first -= shift;
            }
            if (first < 0)
                first = 0;

            return Angle.AtanB(ridge.Points[first], ridge.Points[last]);
        }

        public void Collect(SkeletonBuilder skeleton, FingerprintMinutiaType type, FingerprintTemplate template)
        {
            foreach (SkeletonBuilder.Minutia skeletonMinutia in skeleton.Minutiae)
            {
                if (skeletonMinutia.Valid && skeletonMinutia.Ridges.Count == 1)
                {
                    FingerprintMinutia templateMinutia = new FingerprintMinutia();
                    templateMinutia.Type = type;
                    templateMinutia.Position = skeletonMinutia.Position;
                    templateMinutia.Direction = ComputeDirection(skeletonMinutia.Ridges[0]);
                    template.Minutiae.Add(templateMinutia);
                }
            }
        }
    }
}
