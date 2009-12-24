using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class SkeletonShadow
    {
        public void Draw(SkeletonBuilder skeleton, BinaryMap binary)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
                foreach (SkeletonBuilder.Ridge ridge in minutia.Ridges)
                    if (ridge.Start.Position.Y <= ridge.End.Position.Y)
                        foreach (Point point in ridge.Points)
                            binary.SetBitOne(point);
        }
    }
}
