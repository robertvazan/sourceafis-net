using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Threading.Tasks;
#endif
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public static class ImageIO
    {
#if !COMPACT_FRAMEWORK
        public static byte[,] GetPixels(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, SystemPixelFormat.Format24bppRgb);
            
            byte[] bytes = new byte[height * data.Stride];
            try
            {
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            byte[,] result = new byte[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int offset = (height - 1 - y) * data.Stride + x * 3;
                    result[y, x] = (byte)((bytes[offset + 0] + bytes[offset + 1] + bytes[offset + 2]) / 3);
                }
            return result;
        }

        public static Bitmap GetBitmap(byte[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            Bitmap bmp = new Bitmap(width, height, SystemPixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, SystemPixelFormat.Format24bppRgb);
            
            byte[] bytes = new byte[height * data.Stride];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int offset = (height - 1 - y) * data.Stride + x * 3;
                    bytes[offset + 0] = pixels[y, x];
                    bytes[offset + 1] = pixels[y, x];
                    bytes[offset + 2] = pixels[y, x];
                }

            try
            {
                Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            return bmp;
        }

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
                    int at = (height - y - 1) * width + x;
                    pixels[y, x] = (byte)((flat[at] + flat[at + 1] + flat[at + 2]) / 3);
                }

            return pixels;
        }

        public static BitmapSource Load(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                return decoder.Frames[0];
            }
        }

        public static void Save(BitmapSource image, string filename)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                encoder.Save(stream);
        }
#endif

        public static byte[,] GetInverted(byte[,] image)
        {
            byte[,] result = (byte[,])image.Clone();
            Parallel.For(0, image.GetLength(0), delegate(int y)
            {
                for (int x = 0; x < image.GetLength(1); ++x)
                    result[y, x] = (byte)(255 - image[y, x]);
            });
            return result;
        }
    }
}
