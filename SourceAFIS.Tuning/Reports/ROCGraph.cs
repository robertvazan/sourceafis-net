using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Tuning;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class ROCGraph
    {
        public Size ImageSize = new Size(500, 500);
        public int Precision = 4;
        public ColorF Background = ColorF.White;
        public ColorF Foreground = ColorF.Black;

        public ColorF[,] Draw(ROCCurve ROC)
        {
            ColorF[,] image = new ColorF[ImageSize.Height, ImageSize.Width];
            for (int y = 0; y < ImageSize.Height; ++y)
                for (int x = 0; x < ImageSize.Width; ++x)
                    image[y, x] = Background;

            for (int i = 0; i < ROC.Curve.Count - 1; ++i)
            {
                Point start = ROCPointToPixel(ROC.Curve[i]);
                Point end = ROCPointToPixel(ROC.Curve[i + 1]);
                foreach (Point pixel in Calc.ConstructLine(start, end))
                    image[pixel.Y, pixel.X] = Foreground;
            }

            return image;
        }

        Point ROCPointToPixel(ROCPoint point)
        {
            Point pixel = new Point();
            pixel.X = Convert.ToInt32(ToLogarithmicRange(point.FAR) * (ImageSize.Width - 1));
            pixel.Y = Convert.ToInt32(ToLogarithmicRange(point.FRR) * (ImageSize.Height - 1));
            return pixel;
        }

        double ToLogarithmicRange(float input)
        {
            double log = Math.Log10(input);
            double zeroBase = log + Precision;
            double normalized = zeroBase / Precision;
            double bounded = normalized < 0 ? 0 : normalized;
            return bounded;
        }
    }
}
