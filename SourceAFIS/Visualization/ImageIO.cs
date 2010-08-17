using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Runtime.InteropServices;

namespace SourceAFIS.Visualization
{
    public static class ImageIO
    {
        public static ColorB[,] GetPixels(Bitmap bmp)
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

            ColorB[,] result = new ColorB[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int offset = (height - 1 - y) * data.Stride + x * 3;
                    result[y, x].B = bytes[offset + 0];
                    result[y, x].G = bytes[offset + 1];
                    result[y, x].R = bytes[offset + 2];
                }
            return result;
        }

        public static Bitmap CreateBitmap(ColorB[,] pixels)
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
                    bytes[offset + 0] = pixels[y, x].B;
                    bytes[offset + 1] = pixels[y, x].G;
                    bytes[offset + 2] = pixels[y, x].R;
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

        public static ColorB[,] Load(string filename)
        {
            using (Image fromFile = Bitmap.FromFile(filename))
            {
                using (Bitmap bmp = new Bitmap(fromFile))
                {
                    return GetPixels(bmp);
                }
            }
        }
    }
}
