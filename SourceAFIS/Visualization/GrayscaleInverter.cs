using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Visualization
{
    public class GrayscaleInverter
    {
        public static void Invert(byte[,] image)
        {
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                    image[y, x] = (byte)(255 - image[y, x]);
        }

        public static void Invert(float[,] image)
        {
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                    image[y, x] = -image[y, x];
        }
    }
}
