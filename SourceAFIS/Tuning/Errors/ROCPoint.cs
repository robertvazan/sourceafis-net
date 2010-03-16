using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Errors
{
    public struct ROCPoint
    {
        public float FAR;
        public float FRR;
        public float Threshold;

        public void Average(List<ROCPoint> partial)
        {
            FAR = Calc.Average(partial.ConvertAll<float>(delegate(ROCPoint point) { return point.FAR; }));
            FRR = Calc.Average(partial.ConvertAll<float>(delegate(ROCPoint point) { return point.FRR; }));
            Threshold = Calc.Average(partial.ConvertAll<float>(delegate(ROCPoint point) { return point.Threshold; }));
        }
    }
}
