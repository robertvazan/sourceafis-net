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

        public ColorF(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
