using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public sealed class Calc
    {
        public static int DivRoundUp(int input, int divider)
        {
            return (input + divider - 1) / divider;
        }

        public static float Interpolate(float value0, float value1, float fraction)
        {
            return value0 + fraction * (value1 - value0);
        }

        public static float Interpolate(float topLeft, float topRight, float bottomLeft, float bottomRight, PointF fraction)
        {
            float left = Interpolate(bottomLeft, topLeft, fraction.Y);
            float right = Interpolate(bottomRight, topRight, fraction.Y);
            return Interpolate(left, right, fraction.X);
        }

        public static Point Add(Point left, Point right)
        {
            return left + new Size(right);
        }
    }
}
