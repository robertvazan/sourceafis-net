using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Matching.Minutia;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Visualization
{
    public static class PairingMarkers
    {
        static Point[] Circle = CircleDrawer.Draw(5);

        public static void Draw(ColorF[,] output, IEnumerable<Point> positions)
        {
            foreach (Point point in positions)
            {
                CircleDrawer.Paint(Circle, point, output, ColorF.Red);
            }
        }
    }
}
