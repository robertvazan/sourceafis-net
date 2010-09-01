using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class LinesByOrientation
    {
        [Parameter(Lower = 4, Upper = 128)]
        public int AngularResolution = 32;
        [DpiAdjusted]
        [Parameter(Upper = 50)]
        public int Radius = 5;
        [Parameter(Lower = 1.1, Upper = 4)]
        public float StepFactor = 1.5f;

        public Point[][] Construct()
        {
            Point[][] result = new Point[AngularResolution][];
            for (int orientationIndex = 0; orientationIndex < AngularResolution; ++orientationIndex)
            {
                List<Point> line = new List<Point>();
                line.Add(new Point());
                PointF direction = Angle.ToVector(Angle.ByBucketCenter(orientationIndex, 2 * AngularResolution));
                for (float radius = Radius; radius >= 0.5f; radius /= StepFactor)
                {
                    Point point = Calc.Round(Calc.Multiply(radius, direction));
                    if (!line.Contains(point))
                    {
                        line.Add(point);
                        line.Add(Calc.Negate(point));
                    }
                }
                line.Sort(Calc.CompareYX);
                result[orientationIndex] = line.ToArray();
            }
            return result;
        }
    }
}
