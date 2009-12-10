using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace SourceAFIS.Visualization
{
    public class ImageIO
    {
        public static ColorB[,] GetPixels(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            ColorB[,] result = new ColorB[height, width];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, SystemPixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)data.Scan0;
                    for (int y = 0; y < height; ++y)
                    {
                        byte* scanY = scan0 + y * data.Stride;
                        for (int x = 0; x < width; ++x)
                        {
                            byte* scanXY = scanY + x * 3;
                            int invertedY = height - 1 - y;
                            result[invertedY, x].R = *(scanXY + 0);
                            result[invertedY, x].G = *(scanXY + 1);
                            result[invertedY, x].B = *(scanXY + 2);
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            return result;
        }

        public static Bitmap CreateBitmap(ColorB[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            Bitmap bmp = new Bitmap(width, height, SystemPixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, SystemPixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)data.Scan0;
                    for (int y = 0; y < height; ++y)
                    {
                        byte* scanY = scan0 + y * data.Stride;
                        for (int x = 0; x < width; ++x)
                        {
                            byte* scanXY = scanY + x * 3;
                            int invertedY = height - 1 - y;
                            *(scanXY + 0) = pixels[invertedY, x].R;
                            *(scanXY + 1) = pixels[invertedY, x].G;
                            *(scanXY + 2) = pixels[invertedY, x].B;
                        }
                    }
                }
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
