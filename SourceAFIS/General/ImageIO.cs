using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace SourceAFIS.General
{
    public class ImageIO
    {
        public static byte[,] ToBytes(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            byte[,] result = new byte[height, width];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                        {
                            int value = 0;
                            for (int c = 0; c < 3; ++c)
                                value += *((byte*)data.Scan0 + y * data.Stride + x * 3 + c);
                            result[height - 1 - y, x] = (byte)(255 - value / 3);
                        }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            return result;
        }

        public static byte[,] LoadBytes(string filename)
        {
            using (Image fromFile = Bitmap.FromFile(filename))
            {
                using (Bitmap bmp = new Bitmap(fromFile))
                {
                    return ToBytes(bmp);
                }
            }
        }
    }
}
