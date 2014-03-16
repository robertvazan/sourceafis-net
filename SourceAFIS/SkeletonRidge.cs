using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Utils;

namespace SourceAFIS
{
    class SkeletonRidge
    {
        public readonly IList<Point> Points;
        SkeletonMinutia StartMinutia;
        SkeletonMinutia EndMinutia;

        public readonly SkeletonRidge Reversed;
        
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
            Points = new CircularArray<Point>();
            Reversed = new SkeletonRidge(this);
        }

        SkeletonRidge(SkeletonRidge reversed)
        {
            Reversed = reversed;
            Points = new ReversedList<Point>(reversed.Points);
        }

        public void Detach()
        {
            Start = null;
            End = null;
        }

        public byte ComputeDirection()
        {
            const int segmentLength = 21;
            const int segmentSkip = 1;

            int first = segmentSkip;
            int last = segmentSkip + segmentLength - 1;

            if (last >= Points.Count)
            {
                int shift = last - Points.Count + 1;
                last -= shift;
                first -= shift;
            }
            if (first < 0)
                first = 0;

            return Angle.AtanB(Points[first], Points[last]);
        }
    }
}
