using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public sealed class PixelFormat
    {
        public static byte ToByte(float gray)
        {
            if (gray >= 1)
                return 255;
            else if (gray <= 0)
                return 0;
            else
                return (byte)Convert.ToInt32(gray * 255);
        }

        public static byte ToByte(ColorB color)
        {
            return (byte)((color.R + color.G + color.B + 1) / 3);
        }

        public static byte[,] ToByte(ColorB[,] input)
        {
            byte[,] output = new byte[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToByte(input[y, x]);
            });
            return output;
        }

        static readonly float[] ToFloatTable = CreateToFloatTable();
        
        static float[] CreateToFloatTable()
        {
            float[] result = new float[256];
            for (int i = 0; i < 256; ++i)
                result[i] = i / 255f;
            return result;
        }

        public static float ToFloat(byte gray)
        {
            return ToFloatTable[gray];
        }

        public static float[,] ToFloat(byte[,] input)
        {
            float[,] output = new float[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToFloat(input[y, x]);
            });
            return output;
        }

        public static float ToFloat(ColorF color)
        {
            return (color.R + color.G + color.B) / 3;
        }

        public static ColorB ToColorB(ColorF color)
        {
            return new ColorB(ToByte(color.R), ToByte(color.G), ToByte(color.B));
        }

        public static ColorB[,] ToColorB(ColorF[,] input)
        {
            ColorB[,] output = new ColorB[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToColorB(input[y, x]);
            });
            return output;
        }

        public static ColorF ToColorF(byte gray)
        {
            return ToColorF(ToFloat(gray));
        }

        public static ColorF[,] ToColorF(byte[,] input)
        {
            ColorF[,] output = new ColorF[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToColorF(input[y, x]);
            });
            return output;
        }

        public static ColorF ToColorF(float gray)
        {
            return new ColorF(gray, gray, gray, 1);
        }

        public static ColorF[,] ToColorF(float[,] input)
        {
            ColorF[,] output = new ColorF[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToColorF(input[y, x]);
            });
            return output;
        }

        public static ColorF ToColorF(ColorB color)
        {
            return new ColorF(ToFloat(color.R), ToFloat(color.G), ToFloat(color.B), 1);
        }

        public static ColorF[,] ToColorF(ColorB[,] input)
        {
            ColorF[,] output = new ColorF[input.GetLength(0), input.GetLength(1)];
            Threader.Split(new Range(0, input.GetLength(0)), delegate(int y)
            {
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = ToColorF(input[y, x]);
            });
            return output;
        }
    }
}
