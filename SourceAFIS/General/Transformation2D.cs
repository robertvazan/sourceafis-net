using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public sealed class Transformation2D
    {
        public PointF RotatedX;
        public PointF RotatedY;
        public PointF Move;

        public void Assign(Transformation2D other)
        {
            RotatedX = other.RotatedX;
            RotatedY = other.RotatedY;
            Move = other.Move;
        }

        public PointF Apply(PointF input)
        {
            PointF output = new PointF();
            output.X = RotatedX.X * input.X + RotatedX.Y * input.Y;
            output.Y = RotatedY.X * input.X + RotatedY.Y * input.Y;
            return output + new SizeF(Move);
        }

        public Point Apply(Point input)
        {
            PointF result = Apply(new PointF(input.X, input.Y));
            return new Point(Convert.ToInt32(result.X), Convert.ToInt32(result.Y));
        }
    }
}
