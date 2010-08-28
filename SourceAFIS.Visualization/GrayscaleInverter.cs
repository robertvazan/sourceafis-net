using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class GrayscaleInverter
    {
        public static void Invert(float[,] image)
        {
            Threader.Split(image.GetLength(0), delegate(int y)
            {
                for (int x = 0; x < image.GetLength(1); ++x)
                    image[y, x] = -image[y, x];
            });
        }

        public static float[,] GetInverted(float[,] image)
        {
            float[,] result = (float[,])image.Clone();
            Invert(result);
            return result;
        }
    }
}
