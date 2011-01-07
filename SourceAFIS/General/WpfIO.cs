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
            FormatConvertedBitmap converted = new FormatConvertedBitmap(bitmap, PixelFormats.Gray8, null, 0.5);

            int width = (int)converted.PixelWidth;
            int height = (int)converted.PixelHeight;

            byte[] flat = new byte[width * height];

            converted.CopyPixels(flat, width, 0);

            byte[,] pixels = new byte[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    pixels[y, x] = flat[(height - y - 1) * width + x];

            return pixels;
        }

        public static BitmapSource Load(string filename)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
            image.EndInit();
            return image;
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
