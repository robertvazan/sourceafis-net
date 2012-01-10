using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Extraction.Filters
{
    public static class ImageInverter
    {
        public static byte[,] GetInverted(byte[,] image)
        {
            byte[,] result = new byte[image.GetLength(0), image.GetLength(1)];
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                    result[y, x] = (byte)(255 - image[y, x]);
            return result;
        }
    }
}
