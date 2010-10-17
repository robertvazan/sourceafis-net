using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
#if !COMPACT_FRAMEWORK
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Threading.Tasks;
#endif

namespace SourceAFIS.General
{
    public static class GdiIO
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
