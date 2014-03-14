using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public static class SkeletonShadow
    {
        public static Size GetSize(SkeletonBuilder skeleton)
        {
            RectangleC rect = new RectangleC(0, 0, 1, 1);
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                rect.Include(minutia.Position);
                foreach (SkeletonBuilder.Ridge ridge in minutia.Ridges)
                    if (ridge.Start.Position.Y <= ridge.End.Position.Y)
                        foreach (Point point in ridge.Points)
                            rect.Include(point);
            }
            return rect.Size;
        }

        public static BinaryMap Draw(SkeletonBuilder skeleton)
        {
            BinaryMap binary = new BinaryMap(GetSize(skeleton));
            Draw(skeleton, binary);
            return binary;
        }

        public static void Draw(SkeletonBuilder skeleton, BinaryMap binary)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                binary.SetBitOne(minutia.Position);
                foreach (SkeletonBuilder.Ridge ridge in minutia.Ridges)
                    if (ridge.Start.Position.Y <= ridge.End.Position.Y)
                        foreach (Point point in ridge.Points)
                            binary.SetBitOne(point);
            }
        }
    }
}
