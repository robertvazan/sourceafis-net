using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class AffineTransformer
    {
        public static ColorF[,] Transform(ColorF[,] input, Size outputSize, Transformation2D transformation)
        {
            ColorF[,] output = new ColorF[outputSize.Height, outputSize.Width];
            RectangleC outputRect = new RectangleC(outputSize);
            for (int y = 0; y < input.GetLength(0); ++y)
                for (int x = 0; x < input.GetLength(1); ++x)
                {
                    Point target = transformation.Apply(new Point(x, y));
                    if (outputRect.Contains(target))
                        output[target.Y, target.X] = input[y, x];
                }
            return output;
        }
    }
}
