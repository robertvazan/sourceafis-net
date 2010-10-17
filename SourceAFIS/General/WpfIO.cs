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
            
            byte[] converted = new byte[width * height * 4];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int at = ((height - y - 1) * width + x) * 4;
                    converted[at] = pixels[y, x];
                    converted[at + 1] = pixels[y, x];
                    converted[at + 2] = pixels[y, x];
                }

            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), converted, width * 4, 0, 0);

            return bitmap;
        }

        public static byte[,] GetPixels(BitmapSource bitmap)
        {
            FormatConvertedBitmap converted = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0.5);

            int width = (int)converted.Width;
            int height = (int)converted.Height;

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
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filename);
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
