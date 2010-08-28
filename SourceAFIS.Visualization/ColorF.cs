using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Visualization
{
    public struct ColorF
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public static readonly ColorF Transparent = new ColorF(0, 0, 0, 0);
        public static readonly ColorF Red = new ColorF(1, 0, 0, 1);
        public static readonly ColorF Green = new ColorF(0, 1, 0, 1);
        public static readonly ColorF Blue = new ColorF(0, 0, 1, 1);
        public static readonly ColorF White = new ColorF(1, 1, 1, 1);
        public static readonly ColorF Black = new ColorF(0, 0, 0, 1);

        public ColorF(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorF(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1;
        }
    }
}
