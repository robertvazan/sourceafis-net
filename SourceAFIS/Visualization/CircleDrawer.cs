using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class CircleDrawer
    {
        public static Point[] Draw(int radius)
        {
            int diameter = 2 * radius + 3;
            Point center = new Point(radius + 1, radius + 1);
            bool[,] full = new bool[diameter, diameter];
            for (int y = 0; y < diameter; ++y)
                for (int x = 0; x < diameter; ++x)
                    full[y, x] = Calc.DistanceSq(Calc.Difference(center, new Point(x, y))) <= Calc.Sq(radius + 0.5f);
            
            List<Point> borderPoints = new List<Point>();
            for (int y = 1; y < diameter - 1; ++y)
                for (int x = 1; x < diameter - 1; ++x)
                    if (full[y, x])
                    {
                        bool border = false;
                        foreach (Point relativeNeighbor in Neighborhood.EdgeNeighbors)
                        {
                            Point neighbor = Calc.Add(new Point(x, y), relativeNeighbor);
                            if (!full[neighbor.Y, neighbor.X])
                                border = true;
                        }
                        if (border)
                            borderPoints.Add(Calc.Difference(new Point(x, y), center));
                    }
            return borderPoints.ToArray();
        }

        public static void Paint(Point[] circle, Point center, BinaryMap binary)
        {
            foreach (Point circleRelative in circle)
            {
                Point circlePoint = Calc.Add(center, circleRelative);
                if (binary.Rect.Contains(circlePoint))
                    binary.SetBitOne(circlePoint);
            }
        }

        public static void Paint(Point[] circle, Point center, ColorF[,] output, ColorF color)
        {
            RectangleC rect = new RectangleC(output.GetLength(1), output.GetLength(0));
            foreach (Point circleRelative in circle)
            {
                Point circlePoint = Calc.Add(center, circleRelative);
                if (rect.Contains(circlePoint))
                    output[circlePoint.Y, circlePoint.X] = color;
            }
        }
    }
}
