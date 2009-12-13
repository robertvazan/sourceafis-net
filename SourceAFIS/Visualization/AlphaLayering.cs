using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public class AlphaLayering
    {
        public static void Layer(ColorF[,] bottom, ColorF[,] top)
        {
            for (int y = 0; y < bottom.GetLength(0); ++y)
                for (int x = 0; x < bottom.GetLength(1); ++x)
                {
                    bottom[y, x].R = Calc.Interpolate(bottom[y, x].R, top[y, x].R, top[y, x].A);
                    bottom[y, x].G = Calc.Interpolate(bottom[y, x].G, top[y, x].G, top[y, x].A);
                    bottom[y, x].B = Calc.Interpolate(bottom[y, x].B, top[y, x].B, top[y, x].A);
                    bottom[y, x].A = Calc.Interpolate(bottom[y, x].A, top[y, x].A, top[y, x].A);
                }
        }
    }
}
