using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Runtime.InteropServices;

namespace SourceAFIS.Visualization
{
    public static class ImageSerialization
    {
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
    }
}
