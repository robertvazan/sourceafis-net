using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class GlobalContrast
    {
        public static float[,] GetNormalized(float[,] image)
        {
            float[,] result = (float[,])image.Clone();
            Normalize(result);
            return result;
        }

        public static void Normalize(float[,] image)
        {
            Normalize(image, new RangeF(0, 1));
        }

        public static void Normalize(float[,] image, RangeF newRange)
        {
            RangeF oldRange = GetMinMax(image);
            if (oldRange.Length < 0.000001f)
                oldRange.End = oldRange.Begin + 1f;
            Parallel.For(0, image.GetLength(0), delegate(int y)
            {
                for (int x = 0; x < image.GetLength(1); ++x)
                    image[y, x] = newRange.Interpolate(oldRange.GetFraction(image[y, x]));
            });
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
