using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public class LocalHistogram
    {
        public short[, ,] Analyze(BlockMap blocks, byte[,] image)
        {
            blocks.InitCornerList();
            blocks.InitCornerAreas();
            short[, ,] histogram = new short[blocks.CornerCount.Height, blocks.CornerCount.Width, 256];
            foreach (Point corner in blocks.CornerList)
            {
                RectangleC area = blocks.CornerAreas[corner.Y, corner.X];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        ++histogram[corner.Y, corner.X, image[y, x]];
            }
            return histogram;
        }
    }
}
