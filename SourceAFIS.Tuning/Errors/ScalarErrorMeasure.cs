using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning.Errors
{
    public abstract class ScalarErrorMeasure
    {
        public abstract float Measure(ROCPoint point);

        sealed class OnlyFAR : ScalarErrorMeasure
        {
            public override float Measure(ROCPoint point) { return point.FAR; }
        }

        public static readonly ScalarErrorMeasure FAR = new OnlyFAR();

        sealed class OnlyFRR : ScalarErrorMeasure
        {
            public override float Measure(ROCPoint point) { return point.FRR; }
        }

        public static readonly ScalarErrorMeasure FRR = new OnlyFRR();

        sealed class TakeAverage : ScalarErrorMeasure
        {
            public override float Measure(ROCPoint point) { return (point.FAR + point.FRR) / 2; }
        }

        public static readonly ScalarErrorMeasure Average = new TakeAverage();
    }
}
