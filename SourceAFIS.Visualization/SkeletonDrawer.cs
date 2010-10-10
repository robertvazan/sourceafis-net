using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.Visualization
{
    public static class SkeletonDrawer
    {
        static Point[] Circle = CircleDrawer.Draw(5);

        public static BinaryMap Draw(SkeletonBuilder skeleton, Size size)
        {
            BinaryMap binary = new BinaryMap(size);
            new SkeletonShadow().Draw(skeleton, binary);
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                if (minutia.Valid)
                    CircleDrawer.Paint(Circle, minutia.Position, binary);
            return binary;
        }

        public static BinaryMap DrawEndings(SkeletonBuilder skeleton, Size size)
        {
            BinaryMap binary = new BinaryMap(size);
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                if (minutia.Valid && minutia.Ridges.Count == 1)
                    CircleDrawer.Paint(Circle, minutia.Position, binary);
            return binary;
        }
    }
}
