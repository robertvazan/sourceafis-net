using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public class ScalarColoring
    {
        public static ColorF Interpolate(float gray, ColorF from, ColorF to)
        {
            return new ColorF(
                Calc.Interpolate(from.R, to.R, gray),
                Calc.Interpolate(from.G, to.G, gray),
                Calc.Interpolate(from.B, to.B, gray),
                Calc.Interpolate(from.A, to.A, gray));
        }

        public static ColorF[,] Interpolate(float[,] input, ColorF from, ColorF to)
        {
            ColorF[,] output = new ColorF[input.GetLength(0), input.GetLength(1)];
            for (int y = 0; y < input.GetLength(0); ++y)
                for (int x = 0; x < input.GetLength(1); ++x)
                    output[y, x] = Interpolate(input[y, x], from, to);
            return output;
        }
    }
}
