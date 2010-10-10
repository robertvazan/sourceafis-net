using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace SourceAFIS.Visualization
{
    public static class ImageSerialization
    {
        public static BitmapSource GetBitmapSource(ColorB[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);

            byte[] converted = new byte[width * height * 4];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int at = ((height - y - 1) * width + x) * 4;
                    converted[at] = pixels[y, x].B;
                    converted[at + 1] = pixels[y, x].G;
                    converted[at + 2] = pixels[y, x].R;
                }

            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), converted, width * 4, 0, 0);

            return bitmap;
        }
    }
}
