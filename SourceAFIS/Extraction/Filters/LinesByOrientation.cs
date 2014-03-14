using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class LinesByOrientation
    {
        readonly int AngularResolution = 32;
        readonly int Radius = 7;
        readonly float StepFactor = 1.5f;

        public LinesByOrientation(int resolution = 32, int radius = 7, float step = 1.5f)
        {
            AngularResolution = resolution;
            Radius = radius;
            StepFactor = step;
        }

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
