// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
    class SkeletonRidge
    {
        public readonly SkeletonRidge Reversed;
        public readonly IList<IntPoint> Points;
        SkeletonMinutia StartMinutia;
        SkeletonMinutia EndMinutia;

        public SkeletonMinutia Start
        {
            get { return StartMinutia; }
            set
            {
                if (StartMinutia != value)
                {
                    if (StartMinutia != null)
                    {
                        SkeletonMinutia detachFrom = StartMinutia;
                        StartMinutia = null;
                        detachFrom.DetachStart(this);
                    }
                    StartMinutia = value;
                    if (StartMinutia != null)
                        StartMinutia.AttachStart(this);
                    Reversed.EndMinutia = value;
                }
            }
        }
        public SkeletonMinutia End
        {
            get { return EndMinutia; }
            set
            {
                if (EndMinutia != value)
                {
                    EndMinutia = value;
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
