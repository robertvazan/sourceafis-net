using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public static class WpfIO
    {
#if !COMPACT_FRAMEWORK
        public static BitmapSource GetBitmapSource(byte[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            byte[] flat = new byte[width * height];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    flat[(height - 1 - y) * width + x] = pixels[y, x];
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, flat, width);
        }

        public static byte[,] GetPixels(BitmapSource bitmap)
        {
            FormatConvertedBitmap converted = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0.5);

            int width = (int)converted.PixelWidth;
            int height = (int)converted.PixelHeight;

            byte[] flat = new byte[width * height * 4];

            converted.CopyPixels(flat, width * 4, 0);

            byte[,] pixels = new byte[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int at = ((height - y - 1) * width + x) * 4;
                    pixels[y, x] = (byte)((flat[at] + flat[at + 1] + flat[at + 2]) / 3);
                }

            return pixels;
        }

        public static BitmapSource Load(string filename)
        {
            return new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
        }

        public static void Save(BitmapSource image, string filename)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                encoder.Save(stream);
        }
#endif
    }
}
