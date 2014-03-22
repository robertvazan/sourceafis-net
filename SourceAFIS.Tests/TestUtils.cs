using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SourceAFIS.Tests
{
    class TestUtils
    {
        public static readonly string ImagePath = Path.Combine("..", "..", "..", "..", "TestDatabase");
        public static readonly string TemplatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SourceAFIS.Tests", "Templates");
        public static readonly string[] ImageExtensions = new[] { ".tif", ".png", ".jpg", ".jpeg" };

        public static byte[,] LoadImage(string filename)
        {
            using (Image fromFile = Bitmap.FromFile(filename))
            {
                using (Bitmap bmp = new Bitmap(fromFile))
                {
                    return GetPixels(bmp);
                }
            }
        }

        public static byte[,] GetPixels(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

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
                    int offset = y * data.Stride + x * 3;
                    result[y, x] = (byte)((bytes[offset + 0] + bytes[offset + 1] + bytes[offset + 2]) / 3);
                }
            return result;
        }
    }
}
