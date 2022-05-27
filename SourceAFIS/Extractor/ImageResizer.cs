// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Primitives;

namespace SourceAFIS.Extractor
{
    static class ImageResizer
    {
        static DoubleMatrix Resize(DoubleMatrix input, int newWidth, int newHeight)
        {
            if (newWidth == input.Width && newHeight == input.Height)
                return input;
            var output = new DoubleMatrix(newWidth, newHeight);
            double scaleX = newWidth / (double)input.Width;
            double scaleY = newHeight / (double)input.Height;
            double descaleX = 1 / scaleX;
            double descaleY = 1 / scaleY;
            for (int y = 0; y < newHeight; ++y)
            {
                double y1 = y * descaleY;
                double y2 = y1 + descaleY;
                int y1i = (int)y1;
                int y2i = Math.Min((int)Math.Ceiling(y2), input.Height);
                for (int x = 0; x < newWidth; ++x)
                {
                    double x1 = x * descaleX;
                    double x2 = x1 + descaleX;
                    int x1i = (int)x1;
                    int x2i = Math.Min((int)Math.Ceiling(x2), input.Width);
                    double sum = 0;
                    for (int oy = y1i; oy < y2i; ++oy)
                    {
                        var ry = Math.Min(oy + 1, y2) - Math.Max(oy, y1);
                        for (int ox = x1i; ox < x2i; ++ox)
                        {
                            var rx = Math.Min(ox + 1, x2) - Math.Max(ox, x1);
                            sum += rx * ry * input[ox, oy];
                        }
                    }
                    output[x, y] = sum * (scaleX * scaleY);
                }
            }
            return output;
        }
        public static DoubleMatrix Resize(DoubleMatrix input, double dpi)
        {
            return Resize(input, Doubles.RoundToInt(500.0 / dpi * input.Width), Doubles.RoundToInt(500.0 / dpi * input.Height));
        }
    }
}
