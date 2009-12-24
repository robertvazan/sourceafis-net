using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.Visualization
{
    public sealed class SkeletonDrawer
    {
        static Point[] Circle = CircleDrawer.Draw(5);

        public static BinaryMap Draw(SkeletonBuilder skeleton, Size size)
        {
            BinaryMap binary = new BinaryMap(size);
            new SkeletonShadow().Draw(skeleton, binary);
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                foreach (Point circleRelative in Circle)
                {
                    Point circlePoint = Calc.Add(minutia.Position, circleRelative);
                    if (binary.Rect.Contains(circlePoint))
                        binary.SetBitOne(circlePoint);
                }
            return binary;
        }
    }
}
