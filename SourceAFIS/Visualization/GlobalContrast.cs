using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public class GlobalContrast
    {
        public static void Normalize(float[,] image, RangeF newRange)
        {
            RangeF oldRange = GetMinMax(image);
            if (oldRange.Length < 0.000001f)
                oldRange.End = oldRange.Begin + 1f;
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                    image[y, x] = newRange.Interpolate(oldRange.GetFraction(image[y, x]));
        }

        public static RangeF GetMinMax(float[,] image)
        {
            float min = image[0, 0];
            float max = image[0, 0];
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                {
                    if (image[y, x] < min)
                        min = image[y, x];
                    if (image[y, x] > max)
                        max = image[y, x];
                }
            return new RangeF(min, max);
        }
    }
}
