// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    class SkeletonRidge
    {
        public readonly SkeletonRidge Reversed;
        public readonly IList<IntPoint> Points;
        SkeletonMinutia start;
        SkeletonMinutia end;

        public SkeletonMinutia Start
        {
            get => start;
            set
            {
                if (start != value)
                {
                    if (start != null)
                    {
                        SkeletonMinutia detachFrom = start;
                        start = null;
                        detachFrom.DetachStart(this);
                    }
                    start = value;
                    if (start != null)
                        start.AttachStart(this);
                    Reversed.end = value;
                }
            }
        }
        public SkeletonMinutia End
        {
            get => end;
            set
            {
                if (end != value)
                {
                    end = value;
                    Reversed.Start = value;
                }
            }
        }

        public SkeletonRidge()
        {
            Points = new CircularList<IntPoint>();
            Reversed = new SkeletonRidge(this);
        }
        SkeletonRidge(SkeletonRidge reversed)
        {
            Reversed = reversed;
            Points = new ReversedList<IntPoint>(reversed.Points);
        }

        public void Detach()
        {
            Start = null;
            End = null;
        }
        public double Direction()
        {
            int first = Parameters.RidgeDirectionSkip;
            int last = Parameters.RidgeDirectionSkip + Parameters.RidgeDirectionSample - 1;
            if (last >= Points.Count)
            {
                int shift = last - Points.Count + 1;
                last -= shift;
                first -= shift;
            }
            if (first < 0)
                first = 0;
            return DoubleAngle.Atan(Points[first], Points[last]);
        }
    }
}
