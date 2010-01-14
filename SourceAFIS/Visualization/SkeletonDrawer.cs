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
                if (minutia.Valid)
                    DrawCircle(binary, minutia.Position);
            return binary;
        }

        public static BinaryMap DrawEndings(SkeletonBuilder skeleton, Size size)
        {
            BinaryMap binary = new BinaryMap(size);
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                if (minutia.Valid && minutia.Ridges.Count == 1)
                    DrawCircle(binary, minutia.Position);
            return binary;
        }

        static void DrawCircle(BinaryMap binary, Point center)
        {
            foreach (Point circleRelative in Circle)
            {
                Point circlePoint = Calc.Add(center, circleRelative);
                if (binary.Rect.Contains(circlePoint))
                    binary.SetBitOne(circlePoint);
            }
        }
    }
}
