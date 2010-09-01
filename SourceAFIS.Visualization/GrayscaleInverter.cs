using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public static class GrayscaleInverter
    {
        public static void Invert(float[,] image)
        {
            Parallel.For(0, image.GetLength(0), delegate(int y)
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
